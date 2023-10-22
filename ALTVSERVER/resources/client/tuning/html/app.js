Vue.config.devtools = true;
Vue.prototype.window = window;

const app = new Vue({
  el: '#app',
  data() {
    return {
      data: {
        WheelTypes: [
          {
            Name: 'Sport',
            MaxVariations: 50,
          },
          {
            Name: 'Muscle',
            MaxVariations: 36,
          },
          {
            Name: 'Lowrider',
            MaxVariations: 30,
          },
          {
            Name: 'SUV',
            MaxVariations: 39,
          },
          {
            Name: 'Offroad',
            MaxVariations: 35,
          },
          {
            Name: 'Tuner',
            MaxVariations: 48,
          },
          {
            Name: 'Bike Wheels',
            MaxVariations: 144,
          },
          {
            Name: 'High End',
            MaxVariations: 40,
          },
        ],
        Tuning: {}, /*{
                    VehicleId: null,
                    Staff: false,
                    TargetId: null,
                    CurrentMods: [
                        {
                            Type: 0,
                            Name: 'Spoilers',
                            ModsCount: 5,
                            UnitaryValue: 12000,
                            Value: 0,
                            Current: 0,
                            Selected: 0,
                        },
                        {
                            Type: 1,
                            Name: 'Parachoque Dianteiro',
                            ModsCount: 55,
                            UnitaryValue: 10000,
                            Value: 0,
                            Current: 15,
                            Selected: 15,
                        }
                    ],
                    Mods: [],
                    Repair: 0,
                    RepairValue: 1000,
                    Color1: "#3cdb18",
                    Color2: "#3cab11",
                    CurrentColor1: "#3cdb18",
                    CurrentColor2: "#3cab11",
                    ColorValue: 1235,
                    WheelType: 2,
                    WheelVariation: 13,
                    WheelColor: 127,
                    CurrentWheelType: 2,
                    CurrentWheelVariation: 13,
                    CurrentWheelColor: 127,
                    WheelValue: 2785,
                    NeonColor: "#000000",
                    NeonLeft: 0,
                    NeonRight: 0,
                    NeonFront: 0,
                    NeonBack: 0,
                    CurrentNeonColor: "#000000",
                    CurrentNeonLeft: 0,
                    CurrentNeonRight: 0,
                    CurrentNeonFront: 0,
                    CurrentNeonBack: 0,
                    NeonValue: 5889,
                    HeadlightColor: 3,
                    LightsMultiplier: 4,
                    XenonColorValue: 1748,
                    CurrentHeadlightColor: 3,
                    CurrentLightsMultiplier: 4,
                    WindowTint: 2,
                    CurrentWindowTint: 2,
                    WindowTintValue: 1500,
                    TireSmokeColor: '#000000',
                    CurrentTireSmokeColor: '#000000',
                    TireSmokeColorValue: 157,
                },*/
        Message: '',
      },
    };
  },
  methods: {
    handleChangeMod(i) {
      const mod = this.data.Tuning.CurrentMods[i];
      mod.Selected = parseInt(mod.Selected);

      let multiplier = 1;
      if ((mod.Type == 11 || mod.Type == 12 || mod.Type == 13 || mod.Type == 15 || mod.Type == 16) && mod.Selected > 1)
        multiplier = mod.Selected;

      mod.Value = mod.UnitaryValue * multiplier;

      const x = this.data.Tuning.Mods.findIndex(x => x.Type === mod.Type);

      if (mod.Selected != mod.Current) {
        if (x === -1)
          this.data.Tuning.Mods.push(mod);
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncMod', mod);
    },
    handleChangeRepair() {
      this.data.Tuning.Repair = parseInt(this.data.Tuning.Repair);

      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 255);

      if (this.data.Tuning.Repair == 1) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 255,
            Name: 'Reparo',
            Value: this.data.Tuning.RepairValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }
    },
    handleChangeColor() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 254);

      if (this.data.Tuning.Color1.toUpperCase() != this.data.Tuning.CurrentColor1.toUpperCase()
        || this.data.Tuning.Color2.toUpperCase() != this.data.Tuning.CurrentColor2.toUpperCase()) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 254,
            Name: 'Pintura',
            Value: this.data.Tuning.ColorValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncColor', this.data.Tuning.Color1, this.data.Tuning.Color2);
    },
    handleChangeWheel() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 253);

      this.data.Tuning.WheelType = parseInt(this.data.Tuning.WheelType);
      this.data.Tuning.WheelVariation = parseInt(this.data.Tuning.WheelVariation);
      this.data.Tuning.WheelColor = parseInt(this.data.Tuning.WheelColor);

      if (this.data.Tuning.WheelType != this.data.Tuning.CurrentWheelType
        || this.data.Tuning.WheelVariation != this.data.Tuning.CurrentWheelVariation
        || this.data.Tuning.WheelColor != this.data.Tuning.CurrentWheelColor) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 253,
            Name: 'Rodas',
            Value: this.data.Tuning.WheelValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncWheel', this.data.Tuning.WheelType, this.data.Tuning.WheelVariation, this.data.Tuning.WheelColor);
    },
    handleChangeWheelType() {
      this.data.Tuning.WheelVariation = 0;
      this.handleChangeWheel();
    },
    handleChangeNeon() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 252);

      this.data.Tuning.NeonLeft = parseInt(this.data.Tuning.NeonLeft);
      this.data.Tuning.NeonRight = parseInt(this.data.Tuning.NeonRight);
      this.data.Tuning.NeonFront = parseInt(this.data.Tuning.NeonFront);
      this.data.Tuning.NeonBack = parseInt(this.data.Tuning.NeonBack);

      if (this.data.Tuning.NeonColor.toUpperCase() != this.data.Tuning.CurrentNeonColor.toUpperCase()
        || this.data.Tuning.NeonLeft != this.data.Tuning.CurrentNeonLeft
        || this.data.Tuning.NeonRight != this.data.Tuning.CurrentNeonRight
        || this.data.Tuning.NeonFront != this.data.Tuning.CurrentNeonFront
        || this.data.Tuning.NeonBack != this.data.Tuning.CurrentNeonBack) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 252,
            Name: 'Neon',
            Value: this.data.Tuning.NeonValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncNeon', this.data.Tuning.NeonColor,
          this.data.Tuning.NeonLeft, this.data.Tuning.NeonRight,
          this.data.Tuning.NeonFront, this.data.Tuning.NeonBack);
    },
    handleChangeXenonColor() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 251);

      this.data.Tuning.HeadlightColor = parseInt(this.data.Tuning.HeadlightColor);
      this.data.Tuning.LightsMultiplier = parseInt(this.data.Tuning.LightsMultiplier);

      if (this.data.Tuning.HeadlightColor != this.data.Tuning.CurrentHeadlightColor
        || this.data.Tuning.LightsMultiplier != this.data.Tuning.CurrentLightsMultiplier) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 251,
            Name: 'Cor do Xenon',
            Value: this.data.Tuning.XenonColorValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncXenonColor', this.data.Tuning.HeadlightColor, this.data.Tuning.LightsMultiplier);
    },
    handleChangeWindowTint() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 250);

      this.data.Tuning.WindowTint = parseInt(this.data.Tuning.WindowTint);

      if (this.data.Tuning.WindowTint != this.data.Tuning.CurrentWindowTint) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 250,
            Name: 'Insulfilm',
            Value: this.data.Tuning.WindowTintValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncWindowTint', this.data.Tuning.WindowTint);
    },
    handleChangeTireSmokeColor() {
      const x = this.data.Tuning.Mods.findIndex(x => x.Type === 249);

      if (this.data.Tuning.TireSmokeColor.toUpperCase() != this.data.Tuning.CurrentTireSmokeColor.toUpperCase()) {
        if (x === -1)
          this.data.Tuning.Mods.push({
            Type: 249,
            Name: 'Cor Fumaça Pneus',
            Value: this.data.Tuning.TireSmokeColorValue
          });
      } else {
        if (x !== -1)
          this.data.Tuning.Mods.splice(x, 1);
      }

      if ('alt' in window)
        alt.emit('tuning:SyncTireSmokeColor', this.data.Tuning.TireSmokeColor);
    },
    removeItem(i) {
      const mod = this.data.Tuning.Mods[i];
      if (mod.Type == 255) {
        this.data.Tuning.Repair = 0;
        this.handleChangeRepair();
      } else if (mod.Type == 254) {
        this.data.Tuning.Color1 = this.data.Tuning.CurrentColor1;
        this.data.Tuning.Color2 = this.data.Tuning.CurrentColor2;
        this.handleChangeColor();
      } else if (mod.Type == 253) {
        this.data.Tuning.WheelType = this.data.Tuning.CurrentWheelType;
        this.data.Tuning.WheelVariation = this.data.Tuning.CurrentWheelVariation;
        this.data.Tuning.WheelColor = this.data.Tuning.CurrentWheelColor;
        this.handleChangeWheel();
      } else if (mod.Type == 252) {
        this.data.Tuning.NeonColor = this.data.Tuning.CurrentNeonColor;
        this.data.Tuning.NeonRight = this.data.Tuning.CurrentNeonRight;
        this.data.Tuning.NeonLeft = this.data.Tuning.CurrentNeonLeft;
        this.data.Tuning.NeonFront = this.data.Tuning.CurrentNeonFront;
        this.data.Tuning.NeonBack = this.data.Tuning.CurrentNeonBack;
        this.handleChangeNeon();
      } else if (mod.Type == 251) {
        this.data.Tuning.HeadlightColor = this.data.Tuning.CurrentHeadlightColor;
        this.data.Tuning.LightsMultiplier = this.data.Tuning.CurrentLightsMultiplier;
        this.handleChangeXenonColor();
      } else if (mod.Type == 250) {
        this.data.Tuning.WindowTint = this.data.Tuning.CurrentWindowTint;
        this.handleChangeWindowTint();
      } else if (mod.Type == 249) {
        this.data.Tuning.TireSmokeColor = this.data.Tuning.CurrentTireSmokeColor;
        this.handleChangeTireSmokeColor();
      } else {
        mod.Selected = mod.Current;
        this.handleChangeMod(this.data.Tuning.CurrentMods.indexOf(mod));
      }
    },
    cancel() {
      if ('alt' in window)
        alt.emit('tuning:Done', false, this.data.Tuning);
    },
    confirm() {
      if ('alt' in window)
        alt.emit('tuning:Done', true, this.data.Tuning);
    },
    getTotal() {
      return this.data.Tuning.Mods ? this.data.Tuning.Mods.reduce((a, b) => a + (b['Value'] || 0), 0).toLocaleString('pt-BR') : '0,00';
    },
    closeMessage() {
      this.data.Message = '';
    },
  },
  mounted() {
    if ('alt' in window) {
      alt.on('tuning:SetData', (tuning) => {
        this.data.Tuning = tuning;

        if (this.data.Tuning.VehicleId) {
          for (var i = 0; i < this.data.Tuning.CurrentMods.length; i++)
            this.handleChangeMod(i);

          this.handleChangeRepair();
          this.handleChangeColor();
          this.handleChangeWheel();
          this.handleChangeNeon();
          this.handleChangeXenonColor();
        }
      });

      alt.on('tuning:ShowMessage', (message) => {
        this.data.Message = message;
      });
    }
  }
});