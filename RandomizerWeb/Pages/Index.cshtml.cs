using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomizerApp;
using RandomizerApp.Helpers;
using RandomizerCore.Sprites;
using Z2Randomizer;

namespace RandomizerWeb.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IFormFile RomFile { get; set; }
        [BindProperty] public RandomizerSettings SettingsModel { get; set; } = new RandomizerSettings();
        [BindProperty] public List<ItemPoolItem> SmallDropPool { get; set; } = new List<ItemPoolItem>();
        [BindProperty] public List<ItemPoolItem> LargeDropPool { get; set; } = new List<ItemPoolItem>();

        public string Environment { get; set; }

        public IndexModel(IConfiguration config) {
            Environment = config.GetValue<string>("Environment");
        }

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
    }

    public class ItemPoolItem
    {
        public string Value { get; set; }
        public bool Selected { get; set; }
        public Enums.ItemPool Enum { get; set; }
    }
}
