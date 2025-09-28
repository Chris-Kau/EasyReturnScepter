using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using STool = StardewValley.Tool;

namespace EasyReturnScepter
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;
        private const string returnScepterId = $"{ItemRegistry.type_tool}ReturnScepter";
        private readonly Lazy<STool> ReturnScepter = new(() => ItemRegistry.Create<STool>(returnScepterId));
        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                getValue: () => this.Config.ReturnScepterKeybind,
                setValue: value => this.Config.ReturnScepterKeybind = value,
                name: () => "Keybind (click to change):\n"
            );
        }
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Farmer player = Game1.player;

            if (this.Config.ReturnScepterKeybind.JustPressed())
            {
                STool rs = ReturnScepter.Value;
                if (this.hasReturnScepter(player) && Context.IsPlayerFree && Context.CanPlayerMove)
                {
                    rs.beginUsing(player.currentLocation, (int)player.Position.X, (int)player.Position.Y, player);
                    rs.DoFunction(player.currentLocation, (int)player.Position.X, (int)player.Position.Y, 0, player);
                    rs.endUsing(player.currentLocation, player);
                }
            }
        }

        private bool hasReturnScepter(Farmer player)
        {
            foreach (Item i in player.Items)
            {
                if (i?.QualifiedItemId == returnScepterId)
                    return true;
            }
            return false;
        }
    }
}
