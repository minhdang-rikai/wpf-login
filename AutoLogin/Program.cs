using OfficeOpenXml.Drawing;
using OfficeOpenXml;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows;

namespace AutoLoginApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // phần open app
            string appPath = @"C:\Users\tenla\source\repos\WpfApp1\WpfApp1\bin\Debug\net6.0-windows\WpfApp1.exe";
            System.Diagnostics.Process.Start(appPath);
            Thread.Sleep(2000);

            AutomationElement rootElement = AutomationElement.RootElement;
            AutomationElement loginWindow = null;
            while (loginWindow == null)
            {
                loginWindow = rootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Login"));
                Thread.Sleep(500);
            }

            // chụp lại màn hình khi app mở
            SaveToExcel(CaptureWindow(loginWindow), "LoginScreen_StartApp");

            // phần tự động nhập username, password và click btn login
            AutomationElement usernameTextBox = loginWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "UsernameTextBox"));
            AutomationElement passwordBox = loginWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "PasswordBox"));
            AutomationElement loginButton = loginWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "LoginButton"));

            ValuePattern usernameValuePattern = (ValuePattern)usernameTextBox.GetCurrentPattern(ValuePattern.Pattern);
            usernameValuePattern.SetValue("admin");

            ValuePattern passwordValuePattern = (ValuePattern)passwordBox.GetCurrentPattern(ValuePattern.Pattern);
            passwordValuePattern.SetValue("admin");

            InvokePattern invokePattern = (InvokePattern)loginButton.GetCurrentPattern(InvokePattern.Pattern);
            invokePattern.Invoke();

            Thread.Sleep(1000);
            // chụp lại màn khi sau khi bấm nút login
            SaveToExcel(CaptureWindow(loginWindow), "LoginScreen_AfterClickLogin");
        }

        // chụp ảnh full screen
        //static Bitmap CaptureScreen()
        //{
        //    Bitmap bmpScreen = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
        //                                   System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
        //    using (Graphics g = Graphics.FromImage(bmpScreen))
        //    {
        //      g.CopyFromScreen(System.Windows.Forms.Screen.PrimaryScreen.Bounds.X,
        //                       System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y,
        //                       0, 0,
        //                       bmpScreen.Size,
        //                       CopyPixelOperation.SourceCopy);
        //    }

        //    return bmpScreen;
        //}

        static Bitmap CaptureWindow(AutomationElement windowElement)
        {
            Rect windowRect = windowElement.Current.BoundingRectangle;

            int width = (int)windowRect.Width;
            int height = (int)windowRect.Height;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen((int)windowRect.Left, (int)windowRect.Top, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }

            CaptureChildren(windowElement, bmp);

            return bmp;
        }

        static void CaptureChildren(AutomationElement parentElement, Bitmap bmp)
        {
            AutomationElementCollection children = parentElement.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
            foreach (AutomationElement child in children)
            {
                Rect rect = child.Current.BoundingRectangle;

                if (!rect.IsEmpty && rect.Width > 0 && rect.Height > 0)
                {
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        int x = (int)rect.Left - (int)parentElement.Current.BoundingRectangle.Left;
                        int y = (int)rect.Top - (int)parentElement.Current.BoundingRectangle.Top;

                        Bitmap childBmp = new Bitmap((int)rect.Width, (int)rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (Graphics childGraphics = Graphics.FromImage(childBmp))
                        {
                            childGraphics.CopyFromScreen((int)rect.Left, (int)rect.Top, 0, 0, new System.Drawing.Size((int)rect.Width, (int)rect.Height), CopyPixelOperation.SourceCopy);
                        }

                        graphics.DrawImage(childBmp, x, y);
                    }
                }

                if (TreeWalker.ControlViewWalker.GetFirstChild(child) != null)
                {
                    CaptureChildren(child, bmp);
                }
            }
        }

        static void SaveToExcel(Bitmap screenshot, string screeenName)
        {
            string path = "M:\\Desktop\\Book2.xlsx";

            string tempImagePath = Path.GetTempFileName() + ".png";
            screenshot.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Png);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo(path)))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];

                int startRow = 1;

                if (worksheet.Dimension != null)
                {
                    startRow = worksheet.Dimension.End.Row + 1;
                }

                FileInfo imageFile = new FileInfo(tempImagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture($"screenshot_{startRow}_{DateTime.Now.Ticks}", imageFile);

                worksheet.Cells[startRow, 1].Value = screeenName;
                picture.SetSize(500, 300);
                picture.From.Row = startRow - 1;
                picture.From.Column = 1;
                //picture.SetPosition(startRow, 2);


                worksheet.Cells.AutoFitColumns();
                worksheet.Row(startRow).Height = 300;

                excelPackage.Save();
            }

            File.Delete(tempImagePath);
        }
    }
}
