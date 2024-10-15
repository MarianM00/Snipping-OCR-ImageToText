using ControlzEx.Theming;
using Snipping_OCR_ImageToText.Commands;
using System.Windows.Input;
using Tesseract;
using Application = System.Windows.Application;
using System.IO;

namespace Snipping_OCR_ImageToText.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _ocrText;
        private bool _isDarkTheme;

        public MainViewModel()
        {
            CaptureScreenCommand = new RelayCommand(CaptureScreen);
            CopyTextCommand = new RelayCommand(CopyText);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);

            _isDarkTheme = true;
        }

        public string OCRText
        {
            get { return _ocrText; }
            set
            {
                _ocrText = value;
                RaisePropertyChange();
            }
        }
       
        public ICommand CaptureScreenCommand { get; }
        public ICommand CopyTextCommand { get; }
        public ICommand ToggleThemeCommand { get; }
       
        private void CaptureScreen()
        {
            Application.Current.MainWindow.Hide();

            var snippingForm = new SnippingForm();

            Rectangle combinedBounds = snippingForm.GetCombinedScreenBounds();
            Console.WriteLine($"Combined Screen Bounds: {combinedBounds}");

            snippingForm.StartPosition = FormStartPosition.Manual;
            snippingForm.Location = new System.Drawing.Point(0, 0);
            var dialogResult = snippingForm.ShowDialog(); 

            if (dialogResult == DialogResult.OK)
            {
                var capturedBitmap = snippingForm.CapturedImage;

                OCRText = PerformOCR(capturedBitmap);
            }

            Application.Current.MainWindow.Show();
        }

        public string PerformOCR(Bitmap bitmap)
        {
            
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string tessDataPath = Path.Combine(appPath, "tessdata");
            //string tessDataPath = @"{YourPath}\tessdata-main"; 
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(bitmap))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }

        private void CopyText()
        {
            if (!string.IsNullOrEmpty(OCRText))
            {
                System.Windows.Clipboard.SetText(OCRText);
            }
        }

        private void ToggleTheme()
        {
            _isDarkTheme = !_isDarkTheme;
            if (_isDarkTheme)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Blue");
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
            }
        }
      

    }
}
