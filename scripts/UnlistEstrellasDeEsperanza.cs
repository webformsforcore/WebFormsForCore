// Run with: dotnet run UnlistEstrellasDeEsperanza.cs
// Unlists EstrellasDeEsperanza.WebFormsForCore.Web.Services 1.5.1 from NuGet.org
// (NuGet's DELETE endpoint unlists the package; it does not permanently remove it).

public class Program {
    const string Versions = "1.5.2, 1.5.1, 1.5.0, 1.4.9, 1.4.8, 1.4.7, 1.4.6, 1.4.5, 1.4.4, 1.4.2, 1.4.1, 1.4.0, " +
       "1.3.16, 1.3.15, 1.3.14, 1.3.13, 1.3.12, 1.3.11, 1.3.10, 1.3.9, 1.3.8, 1.3.7, 1.3.6, 1.3.5, 1.3.4, 1.3.3, 1.3.2, 1.3.1, 1.3.0, " +
       "1.2.9, 1.2.8, 1.2.7, 1.2.6, 1.2.5, 1.2.4, 1.2.2, 1.2.1, 1.2.0, 1.1.3, 1.1.1-beta, 1.1.0-beta, 1.0.0";
    const string Packages = "Web, Web.Services, Web.RegularExpressions, Build, Web.ApplicationServices, Web.Extensions, " +
        "Serialization.Formatters, Configuration, Drawing, Compilers, Web.Mobile, Web.Infrastructure, WebGrease, Web.Optimization, " +
        "AjaxControlToolkit, Web.Optimization.WebForms, AjaxControlToolkit.StaticResources, AjaxControlToolkit.HtmlEditor.Sanitizer";
    const int Delay = 15000;

    const string StartVersion = "1.3.17";
    static int TotalPackages = (Versions.Count(ch => ch == ',') + 1) * (Packages.Count(ch => ch == ',') + 1); 

    public static async Task Main(string[] args) {
        var apiKey = File.ReadAllText(@"..\..\..\..\NugetApiKey.txt").Trim();

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-NuGet-ApiKey", apiKey);

        bool success = true;
        var startVersion = new Version(StartVersion);
        int n = 0;
        foreach (var version in Versions.Split(',', StringSplitOptions.TrimEntries))
        {
            if (Version.TryParse(version, out Version? ver) && ver > startVersion) continue;

            foreach (var package in Packages.Split(',')
                .Select(p => "EstrellasDeEsperanza.WebFormsForCore." + p.Trim()))
            {
                var url = $"https://www.nuget.org/api/v2/package/{package}/{version}";

                Console.WriteLine($"DELETE {url}");
                var response = await client.DeleteAsync(url);
                if (!response.IsSuccessStatusCode) {
                    Console.WriteLine($"Error: {(int)response.StatusCode} {response.StatusCode}");
                    success = false;
                }
                var body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body)) Console.WriteLine(body);

                n++;
                Console.WriteLine($"{(n * 100) / TotalPackages} %");

                Thread.Sleep(Delay);
            }
        }

        Environment.Exit(success ? 0 : 1);
    }
}