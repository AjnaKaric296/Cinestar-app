using QRCoder;
using System.IO;
using Microsoft.Maui.Controls;

namespace Cinestar_app.Helpers
{
    public static class QrCodeHelper
    {
        public static ImageSource GenerateQrCode(string url)
        {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var bytes = qrCode.GetGraphic(20);

            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
    }
}
