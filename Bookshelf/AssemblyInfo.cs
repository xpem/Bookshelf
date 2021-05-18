using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

//as fontes devem estar como "recursos inseridos"
[assembly: ExportFont("Font Awesome 5 Free-Regular-400.otf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "FontAwesomeBold")]
[assembly: ExportFont("Font Awesome 5 Brands-Regular-400.otf", Alias = "FontAwesomeBrands")]