using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomizerApp;
using RandomizerApp.Helpers;
using Z2Randomizer;

namespace RandomizerWeb.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IFormFile RomFile { get; set; }
        [BindProperty] public IFormFile JsonFile { get; set; }
        [BindProperty] public string InputFlags { get; set; }
        [BindProperty] public RandomizerSettings SettingsModel { get; set; } = new RandomizerSettings();
        [BindProperty] public List<ItemPoolItem> SmallDropPool { get; set; } = new List<ItemPoolItem>();
        [BindProperty] public List<ItemPoolItem> LargeDropPool { get; set; } = new List<ItemPoolItem>();

        public void OnGet()
        {
            PopulateItemDropPool();
        }

        private void PopulateItemDropPool()
        {
            //kinda hacky, probably a better way to do this
            foreach (var item in Enum.GetValues(typeof(Enums.ItemPool)).Cast<Enums.ItemPool>())
            {
                SmallDropPool.Add(new ItemPoolItem
                {
                    Value = item.GetStringValue(),
                    Selected = SettingsModel.SmallEnemyPool.Contains(item),
                    Enum = item
                });
                LargeDropPool.Add(new ItemPoolItem
                {
                    Value = item.GetStringValue(),
                    Selected = SettingsModel.LargeEnemyPool.Contains(item),
                    Enum = item
                });
            }
        }

        public void OnGetPreset(string id)
        {
            PopulateItemDropPool();
            try
            {
                SettingsModel =
                    RandomizerSettingsFactory.Generate((Enums.Presets)Enum.Parse(typeof(Enums.Presets), id));
            }
            catch
            {
                //
            }
        }

        public IActionResult OnPostUpload()
        {
            if (SmallDropPool.Any(x => x.Selected))
                SettingsModel.SmallEnemyPool = SmallDropPool.Where(x => x.Selected).Select(x => x.Enum).ToList();
            if (LargeDropPool.Any(x => x.Selected))
                SettingsModel.LargeEnemyPool = LargeDropPool.Where(x => x.Selected).Select(x => x.Enum).ToList();

            //disabled values don't post
            if (!SettingsModel.ShufflePalaceRooms && !SettingsModel.ThunderbirdRequired)
                SettingsModel.ThunderbirdRequired = true;

            if (SettingsModel.ShuffleAllExperienceNeeded)
            {
                SettingsModel.ShuffleAttackExperienceNeeded = true;
                SettingsModel.ShuffleMagicExperienceNeeded = true;
                SettingsModel.ShuffleLifeExperienceNeeded = true;
            }

            using (var fileStream = RomFile.OpenReadStream())
            {
                //generate rom, json
                var romGenerator = new Hyrule(SettingsModel.GetRandomizerProperties(RomFile.FileName, fileStream));
                var rom = romGenerator.GenerateRomStream();

                return new FileStreamResult(rom.OutputStream, "application/octet-stream") { FileDownloadName = rom.FileName };

            }
        }
        public async Task OnPostJson()
        {
            PopulateItemDropPool();
            
            var json = "";
            using (var fileStream = JsonFile.OpenReadStream())
            using (var reader = new StreamReader(fileStream))
            {
                json = await reader.ReadToEndAsync();
            }

            SettingsModel = JsonConvert.DeserializeObject<RandomizerSettings>(json);
        }

        public void OnPostFlags() {
            var randomizerSettings = new RandomizerSettings();
            randomizerSettings.GenerateFromFlags(InputFlags);
        }

        public IActionResult OnPostGetJson()
        {
            return new FileStreamResult(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(SettingsModel.ToJson())), "application/json");
        }
    }

    public class ItemPoolItem
    {
        public string Value { get; set; }
        public bool Selected { get; set; }
        public Enums.ItemPool Enum { get; set; }
    }
}
