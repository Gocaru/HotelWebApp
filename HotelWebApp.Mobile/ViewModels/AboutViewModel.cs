using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string appName = "Hotel Management";

        [ObservableProperty]
        private string appVersion = "1.0.0";

        [ObservableProperty]
        private string releaseDate = "October 2025";

        [ObservableProperty]
        private string author = "Gonçalo Russo";
    }
}
