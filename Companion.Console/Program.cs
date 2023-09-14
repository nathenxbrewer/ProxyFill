// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Drawing;
using System.Net.Mime;
using Companion.Console;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using ProxyFill.Model;
using ProxyFill.Shared.ViewModel;
using File = System.IO.File;

var path = "";
if (args.Length != 1)
{
    Console.WriteLine("Specify the path to your ProxyFill list:");
    path = Console.ReadLine();
}
else
{
    path = args[0];
}

var listString = File.ReadAllText(path);
var cardList = JsonConvert.DeserializeObject<List<ProxyCardDTO>>(listString);
cardList = cardList.Where(x => x.FrontImage != null).ToList();
DownloadImages(cardList).Wait();


Console.WriteLine(args[0]);
Console.ReadLine();

static async Task DownloadImages(List<ProxyCardDTO> cards)
{
    if (!Directory.Exists("Images"))
    {
        Directory.CreateDirectory("Images");
    }
    //group by imageURL for duplicates, they may have multiple cards with different proxy art, so we cant group by card. 
    var groupedList = cards.GroupBy(x => x.FrontImage);
    int x = 0;
    using (var client = new HttpClient())
    {
        foreach (var group in groupedList)
        {
            var card = group.Select(x => x).First();
            var filename = $"{card.Name} {card.SetCode} {card.Number}.png";
            var path = $"Images/{filename}";

            if (File.Exists(path))
            {
                x++;
                Console.Clear();
                Console.WriteLine($"{filename} already exists, continuing...");
                ProgressBar(x,groupedList.Count());
                continue;
            }

            var downloadLink = group.Key;
            if (downloadLink.Contains("https://drive.google.com/uc?"))
            {
                var fileId = downloadLink.Substring(47);
                downloadLink =
                    $"https://content.googleapis.com/drive/v2/files/{fileId}?key=AIzaSyBOGAtxTDZMJas_EkIRb0pVBpyQYpTaHXU&alt=media&source=downloadUrl";
            }
            using (var result = await client.GetAsync(group.Key))
            {
                if (result.IsSuccessStatusCode)
                {
                    var imageData = await result.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(path, imageData);
                    //validate image is there
                    if (File.Exists(path))
                    {
                        x++;
                        Console.Clear();
                        Console.WriteLine($"Downloaded {filename} successfully!");
                        ProgressBar(x,groupedList.Count());
                    }
                }
            }
        }
    }
}

static void AutoFill()
{
    var driver = new EdgeDriver(@"/Users/nathenbrewer/Downloads");
    driver.Url = "https://www.drivethrucards.com/login.php";
    var orLogInLink = driver.FindElement(By.XPath("//*[@id=\"create_account_box\"]/div[2]/span/span[1]/a"));
    orLogInLink.Click();
    driver.FindElement(By.Id("login_email_address")).SendKeys("email");
    driver.FindElement(By.Id("login_password")).SendKeys("password");
    driver.FindElement(By.Id("loginbutton")).Click();
    Thread.Sleep(TimeSpan.FromSeconds(3));
    driver.Navigate().GoToUrl("https://www.drivethrucards.com/builder/deck/images/back/64baff58d1078");
//driver.Navigate().GoToUrl("https://www.drivethrucards.com/builder/deck/images/back/");
    var uploadArea = driver.FindElement(By.XPath("//*[@id=\"upload\"]/div[6]/div/div[2]/div[1]"));
    driver.ExecuteScript("arguments[0].style.display = 'flex';", uploadArea);
    var uploadBox = driver.FindElement(By.Id("upload_upload"));
    var path = Path.GetFullPath("//Users/nathenbrewer//Downloads//Pokemon MPC Back.png");
//uploadBox.SendKeys(path);
//Helper.DropFile(driver, uploadBox, path);


    var dropper = driver.FindElement(By.XPath("//*[@id=\"upload\"]/div[6]/div/div[2]"));
    Helper.DropFile(driver, dropper, path);




//var uploadButton = driver.FindElement(By.XPath("//*[@id=\"upload\"]/div[6]/div/div[2]/div[1]/input[2]"));
//uploadButton.Click();

//wait to see result in table
    Console.ReadLine();
//https://www.drivethrucards.com/login.php
//https://www.drivethrucards.com/index.php
//https://www.drivethrucards.com/builder/deck/images/back/
//name upload_upload
//table class=uploaded-images table

}

// Repeat
static string Repeat(string str, int times) {
    return string.Concat(Enumerable.Repeat(str, times));
}

// Progress Bar
static void ProgressBar(int progress, int total, int chunks = 30, ConsoleColor completeColour = ConsoleColor.Green, ConsoleColor remainingColour = ConsoleColor.Gray, string symbol = "■", bool showPercent = true) {
    //Draw Blank Progress Bar
    Console.CursorLeft = 0;
    Console.Write("  [");
    Console.CursorLeft = chunks + 3;
    Console.Write("]");
    Console.CursorLeft = 3;
    float chunk = (float) chunks / total;

    // Chunk Calculations
    double completeRaw = Math.Ceiling((double) chunk * progress);
    int complete = (int) Math.Ceiling((double) chunk * progress);
    int remaining = chunks - complete;

    // Draw Progress
    Console.ForegroundColor = completeColour;
    Console.Write(Repeat(symbol, complete));
    Console.ForegroundColor = remainingColour;
    Console.Write(Repeat(symbol, remaining));

    // Show Percent
    Console.CursorLeft = chunks + 4;
    Console.ResetColor();
    if (showPercent) {
        int percent = (int) ((float) progress / (float) total * 100);
        Console.Write($" {Repeat(" ", 3 - percent.ToString().Length)}{percent} %");
    }
}
