using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp1.mvvm.ViewModels;
using Path = System.Windows.Shapes.Path;

namespace WpfApp1
{
    public partial class Window10 : Window
    {
        private readonly Window10ViewModel _vm;
        private bool _modeSombre = false;

        public Window10()
        {
            InitializeComponent();

            _vm = new Window10ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); },
                GraphiquesCharges = () =>
                {
                    ChargerDonut();
                    ChargerEvolution();
                    ChargerMouvements();
                }
            };
            DataContext = _vm;

            this.Loaded += Window10_Loaded;

            NavAccueil.Checked += (s, e) => _vm.NavAccueilCommand.Execute(null);
            NavStock.Checked += (s, e) => _vm.NavStockCommand.Execute(null);
            NavMateriel.Checked += (s, e) => _vm.NavMaterielCommand.Execute(null);
            NavConsommable.Checked += (s, e) => _vm.NavConsommableCommand.Execute(null);
            NavPiece.Checked += (s, e) => _vm.NavPieceCommand.Execute(null);
            NavCommande.Checked += (s, e) => _vm.NavCommandeCommand.Execute(null);
        }

        private void Window10_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.Initialize();
        }

        // ====== دسم الـ Canvas مباشرة (منطق View بحت، بحال AddCol فـ Window8) ======

        private void ChargerDonut()
        {
            try
            {
                DonutCanvas.Children.Clear();
                int total = _vm.KpiMateriel + _vm.KpiConsommable + _vm.KpiPiece;
                if (total == 0) total = 1;

                double pMat = (double)_vm.KpiMateriel / total;
                double pConso = (double)_vm.KpiConsommable / total;
                double pPiece = (double)_vm.KpiPiece / total;

                var center = new Point(75, 75);
                double radius = 52;
                double thickness = 14;
                double startAngle = -90;

                startAngle = AjouterSegmentDonut(DonutCanvas, center, radius, thickness, startAngle, pMat * 360, (Brush)FindResource("Blue"));
                startAngle = AjouterSegmentDonut(DonutCanvas, center, radius, thickness, startAngle, pConso * 360, (Brush)FindResource("Green"));
                AjouterSegmentDonut(DonutCanvas, center, radius, thickness, startAngle, pPiece * 360, (Brush)FindResource("Orange"));
            }
            catch { }
        }

        private double AjouterSegmentDonut(Canvas canvas, Point center, double radius, double thickness, double startAngle, double sweepAngle, Brush brush)
        {
            if (sweepAngle <= 0.001) return startAngle;
            double endAngle = startAngle + sweepAngle;
            Point p1 = PointSurCercle(center, radius, startAngle);
            Point p2 = PointSurCercle(center, radius, endAngle);

            var figure = new PathFigure { StartPoint = p1, IsClosed = false };
            figure.Segments.Add(new ArcSegment(p2, new Size(radius, radius), 0, sweepAngle > 180, SweepDirection.Clockwise, true));
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new Path { Data = geometry, Stroke = brush, StrokeThickness = thickness, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
            canvas.Children.Add(path);
            return endAngle;
        }

        private Point PointSurCercle(Point center, double radius, double angleDeg)
        {
            double angleRad = angleDeg * Math.PI / 180.0;
            return new Point(center.X + radius * Math.Cos(angleRad), center.Y + radius * Math.Sin(angleRad));
        }

        private void ChargerEvolution()
        {
            try
            {
                EvolutionCanvas.Children.Clear();
                var mois = _vm.EvolutionMois;
                var valeurs = _vm.EvolutionValeurs;

                double width = 520, height = 150, paddingX = 30, paddingBottom = 25, paddingTop = 20;
                int max = valeurs.Count > 0 && valeurs.Max() > 0 ? valeurs.Max() : 10;

                double availableWidth = width - paddingX * 2;
                double availableHeight = height - paddingBottom - paddingTop;
                double stepX = availableWidth / (mois.Count - 1);

                var points = new System.Collections.Generic.List<Point>();
                for (int i = 0; i < valeurs.Count; i++)
                {
                    double x = paddingX + i * stepX;
                    double y = height - paddingBottom - (valeurs[i] / (double)max * availableHeight);
                    points.Add(new Point(x, y));
                }

                if (points.Count == 0) return;

                var areaGeometry = new PathGeometry();
                var areaFigure = new PathFigure { StartPoint = new Point(points[0].X, height - paddingBottom), IsClosed = true };
                areaFigure.Segments.Add(new LineSegment(points[0], false));

                for (int i = 0; i < points.Count - 1; i++)
                {
                    Point pStart = points[i];
                    Point pEnd = points[i + 1];
                    double controlX1 = pStart.X + stepX / 2;
                    double controlX2 = pEnd.X - stepX / 2;

                    areaFigure.Segments.Add(new BezierSegment(new Point(controlX1, pStart.Y), new Point(controlX2, pEnd.Y), pEnd, true));
                }
                areaFigure.Segments.Add(new LineSegment(new Point(points[points.Count - 1].X, height - paddingBottom), false));
                areaGeometry.Figures.Add(areaFigure);

                var areaGradient = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                areaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(50, 46, 123, 246), 0.0));
                areaGradient.GradientStops.Add(new GradientStop(Color.FromArgb(0, 46, 123, 246), 1.0));

                var areaPath = new Path { Data = areaGeometry, Fill = areaGradient };
                EvolutionCanvas.Children.Add(areaPath);

                var lineGeometry = new PathGeometry();
                var lineFigure = new PathFigure { StartPoint = points[0], IsClosed = false };

                for (int i = 0; i < points.Count - 1; i++)
                {
                    Point pStart = points[i];
                    Point pEnd = points[i + 1];
                    double controlX1 = pStart.X + stepX / 2;
                    double controlX2 = pEnd.X - stepX / 2;

                    lineFigure.Segments.Add(new BezierSegment(new Point(controlX1, pStart.Y), new Point(controlX2, pEnd.Y), pEnd, true));
                }
                lineGeometry.Figures.Add(lineFigure);

                var strokePath = new Path { Data = lineGeometry, Stroke = (Brush)FindResource("Blue"), StrokeThickness = 3 };
                EvolutionCanvas.Children.Add(strokePath);

                for (int i = 0; i < points.Count; i++)
                {
                    var dot = new Ellipse { Width = 8, Height = 8, Fill = Brushes.White, Stroke = (Brush)FindResource("Blue"), StrokeThickness = 2.5 };
                    Canvas.SetLeft(dot, points[i].X - 4);
                    Canvas.SetTop(dot, points[i].Y - 4);
                    EvolutionCanvas.Children.Add(dot);

                    var lbl = new TextBlock { Text = mois[i], FontSize = 11, Foreground = (Brush)FindResource("TextMuted"), FontWeight = FontWeights.SemiBold };
                    lbl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(lbl, points[i].X - (lbl.DesiredSize.Width / 2));
                    Canvas.SetTop(lbl, height - 18);
                    EvolutionCanvas.Children.Add(lbl);
                }
            }
            catch { }
        }

        private void ChargerMouvements()
        {
            try
            {
                EtatCanvas.Children.Clear();
                var entrees = _vm.MouvementsEntrees;
                var sorties = _vm.MouvementsSorties;
                var joursDynamiques = _vm.MouvementsJours;

                if (joursDynamiques.Count == 0) return;

                double width = 520, height = 140, paddingX = 30, paddingBottom = 25, paddingTop = 15;
                int max = Math.Max(entrees.Max(), sorties.Max());
                if (max == 0) max = 5;

                double availableWidth = width - paddingX * 2;
                double availableHeight = height - paddingBottom - paddingTop;
                double stepX = availableWidth / (joursDynamiques.Count - 1);
                double barWidth = 10;

                for (int i = 0; i < joursDynamiques.Count; i++)
                {
                    double centerX = paddingX + i * stepX;
                    double hEntree = (entrees[i] / (double)max) * availableHeight;
                    double hSortie = (sorties[i] / (double)max) * availableHeight;

                    if (hEntree < 1.5) hEntree = 1.5;
                    if (hSortie < 1.5) hSortie = 1.5;

                    var barEntree = new Border { Width = barWidth, Height = hEntree, Background = (Brush)FindResource("Green"), CornerRadius = new CornerRadius(5, 5, 0, 0) };
                    Canvas.SetLeft(barEntree, centerX - barWidth - 2);
                    Canvas.SetTop(barEntree, height - paddingBottom - hEntree);
                    EtatCanvas.Children.Add(barEntree);

                    var barSortie = new Border { Width = barWidth, Height = hSortie, Background = (Brush)FindResource("Red"), CornerRadius = new CornerRadius(5, 5, 0, 0) };
                    Canvas.SetLeft(barSortie, centerX + 2);
                    Canvas.SetTop(barSortie, height - paddingBottom - hSortie);
                    EtatCanvas.Children.Add(barSortie);

                    var lbl = new TextBlock { Text = joursDynamiques[i], FontSize = 11, Foreground = (Brush)FindResource("TextMuted"), FontWeight = FontWeights.SemiBold };
                    lbl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(lbl, centerX - (lbl.DesiredSize.Width / 2));
                    Canvas.SetTop(lbl, height - 18);
                    EtatCanvas.Children.Add(lbl);
                }
            }
            catch { }
        }

        // ====== Export (كيحتاج RenderTargetBitmap مباشرة على الـ View) ======

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (BtnExport.ContextMenu != null)
            {
                BtnExport.ContextMenu.PlacementTarget = BtnExport;
                BtnExport.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                BtnExport.ContextMenu.IsOpen = true;
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Classeur Excel (*.xlsx)|*.xlsx", FileName = "Rapport_Stock_Complet_Airbus.xlsx" };
                if (dialog.ShowDialog() != true) return;

                using (var workbook = new XLWorkbook())
                using (var conn = WpfApp1.mvvm.Services.DatabaseService.GetConnection())
                {
                    conn.Open();

                    // ===== Feuille 1 : Résumé Statistiques =====
                    var wsDash = workbook.Worksheets.Add("Résumé Statistiques");
                    wsDash.Cell(1, 1).Value = "MOUVEMENT DU STOCK IT AIRBUS";
                    wsDash.Cell(1, 1).Style.Font.Bold = true;
                    wsDash.Cell(1, 1).Style.Font.FontSize = 14;

                    wsDash.Cell(4, 1).Value = "Catégorie";
                    wsDash.Cell(4, 2).Value = "Quantité Totale";
                    wsDash.Cell(4, 1).Style.Font.Bold = true;
                    wsDash.Cell(4, 2).Style.Font.Bold = true;
                    wsDash.Cell(5, 1).Value = "Matériels"; wsDash.Cell(5, 2).Value = _vm.KpiMateriel;
                    wsDash.Cell(6, 1).Value = "Consommables"; wsDash.Cell(6, 2).Value = _vm.KpiConsommable;
                    wsDash.Cell(7, 1).Value = "Pièces de rechange"; wsDash.Cell(7, 2).Value = _vm.KpiPiece;
                    wsDash.Cell(8, 1).Value = "Commandes en attente"; wsDash.Cell(8, 2).Value = _vm.KpiCommandes;
                    wsDash.Columns().AdjustToContents();

                    // ===== Feuille 2 : Matériel =====
                    EcrireFeuilleDonnees(workbook, "Matériel", conn,
                        "SELECT etiquette, type_materiel, nom, marque, modele, num_serie, stockage, RAM, processeur, adr_mac, date_ajout FROM materiel");

                    // ===== Feuille 3 : Consommables =====
                    EcrireFeuilleDonnees(workbook, "Consommables", conn,
                        "SELECT id, modele, couleur, reference, quantite, date_ajout FROM consommable");

                    // ===== Feuille 4 : Pièces de rechange =====
                    EcrireFeuilleDonnees(workbook, "Pièces de rechange", conn,
                        "SELECT id, modele, piece, quantite, date_ajout FROM piece_de_rechange");

                    // ===== Feuille 5 : Mouvements de stock =====
                    EcrireFeuilleDonnees(workbook, "Mouvements de stock", conn,
                        "SELECT type_mvt, table_source, quantite, date_mvt FROM mvt_stock ORDER BY date_mvt DESC");

                    // ===== Feuille 6 : Commandes =====
                    try
                    {
                        EcrireFeuilleDonnees(workbook, "Commandes", conn,
                            "SELECT type_pc, service, demandeur, beneficiaire, commentaire, statut, date_commande FROM commande ORDER BY date_commande DESC");
                    }
                    catch
                    {
                        var wsCmd = workbook.Worksheets.Add("Commandes");
                        wsCmd.Cell(1, 1).Value = "Table 'commande' introuvable.";
                    }

                    workbook.SaveAs(dialog.FileName);
                }

                MessageBox.Show("Exportation Excel réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show("Erreur Excel: " + ex.Message); }
        }

        // ====== دالة مساعدة: كتابة عادية خلية بخلية، بلا Excel Table/فلاتر ======
        private void EcrireFeuilleDonnees(XLWorkbook workbook, string nomFeuille, MySqlConnection conn, string requete)
        {
            var ws = workbook.Worksheets.Add(nomFeuille);
            var dt = new DataTable();
            using (var adapter = new MySqlDataAdapter(requete, conn))
                adapter.Fill(dt);

            for (int col = 0; col < dt.Columns.Count; col++)
            {
                var cell = ws.Cell(1, col + 1);
                cell.Value = dt.Columns[col].ColumnName;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0B3D6B");
                cell.Style.Font.FontColor = XLColor.White;
            }

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    var value = dt.Rows[row][col];
                    ws.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                }
            }

            ws.Columns().AdjustToContents();
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Fichier PDF (*.pdf)|*.pdf", FileName = "Rapport_Dashboard_Airbus.pdf" };
                if (dialog.ShowDialog() == true)
                {
                    FrameworkElement visual = MainChartsContainer;
                    if (visual.ActualWidth == 0 || visual.ActualHeight == 0) return;

                    var bmp = new System.Windows.Media.Imaging.RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                    bmp.Render(visual);

                    var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

                    byte[] imageBytes;
                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        imageBytes = ms.ToArray();
                    }

                    var doc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                    using (var fs = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        var pdfImage = iTextSharp.text.Image.GetInstance(imageBytes);
                        pdfImage.ScaleToFit(doc.PageSize.Width - 20f, doc.PageSize.Height - 20f);
                        pdfImage.Alignment = Element.ALIGN_CENTER;
                        doc.Add(pdfImage);
                        doc.Close();
                    }
                    MessageBox.Show("Rapport PDF généré avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show("Erreur PDF: " + ex.Message); }
        }

        // ====== Thème (كيبدل Background ديال الـ Window مباشرة، View بحت) ======
        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            _modeSombre = !_modeSombre;
            this.Background = _modeSombre ? new SolidColorBrush(Color.FromRgb(20, 27, 38)) : (Brush)FindResource("Bg");
            if (BtnTheme.Content is MaterialDesignThemes.Wpf.PackIcon icon)
                icon.Kind = _modeSombre ? MaterialDesignThemes.Wpf.PackIconKind.WeatherNight : MaterialDesignThemes.Wpf.PackIconKind.WeatherSunny;
        }
    }
}