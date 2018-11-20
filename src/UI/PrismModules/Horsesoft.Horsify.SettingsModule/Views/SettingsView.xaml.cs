using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Horsesoft.Horsify.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ChangeSkinColorButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;   
            SwitchSkinColor(button.CommandParameter.ToString());
        }

        private void SwitchSkinColor(string skinDictionary)
        {
            var mergedDicts = Application.Current.Resources.MergedDictionaries;
            var currentSkin = mergedDicts.Where(x => x.Source.ToString().Contains("Skin") && !x.Source.ToString().Contains("Generic"))
                .FirstOrDefault();
            if (currentSkin != null)
            {
                mergedDicts.Remove(currentSkin);

                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(Path.Combine(@"/Horsesoft.Horsify.Resource;component/", skinDictionary.Replace("\\\\", "\\")), UriKind.RelativeOrAbsolute);

                mergedDicts.Add(dict);
            }
        }
    }
}
