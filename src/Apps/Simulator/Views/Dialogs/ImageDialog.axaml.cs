using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using OpenExamSuite.Simulator.Localization;

namespace OpenExamSuite.Simulator.Views.Dialogs;

/// <summary>
/// An enlarged question image. The window can be resized and the image always scales to fit.
/// </summary>
public partial class ImageDialog : Window
{
    public ImageDialog()
        : this(null)
    {
    }

    public ImageDialog(Bitmap? image)
    {
        InitializeComponent();
        Title = Strings.Get("Exam_ImageTitle");
        Picture.Source = image;
        CloseButton.Click += (_, _) => Close();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
                Close();
        };
        Opened += (_, _) => CloseButton.Focus();
    }
}
