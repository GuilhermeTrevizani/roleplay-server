Vue.component('tab-decor', {
  props: ['data'],
  methods: {
    handleChange() {
      this.$root.$emit('updateCharacter');
    },
    getOverlayColorCount() {
      return 59;
    }
  },
  template: `
        <div class="options">
            <div v-for="(name, i) in colorOverlays" :key="i" class="option">
                <div class="labelContainer">
                    <div class="label">
                        {{ colorOverlays[i].label }}
                    </div>
                    <div class="value">
                        {{ data.info.ColorOverlays[i].Value }} | {{ colorOverlays[i].max }}
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" :min="colorOverlays[i].min" :max="colorOverlays[i].max" v-model.number="data.info.ColorOverlays[i].Value" :step="colorOverlays[i].increment" @input="e => handleChange()" />
                </div>

                <div class="labelContainer">
                    <div class="label">
                        {{ colorOverlays[i].label }} Opacidade
                    </div>
                    <div class="value">
                        {{ parseFloat(data.info.ColorOverlays[i].Opacity).toFixed(1) }} | 1.0
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" :min="0" :max="1" v-model.number="data.info.ColorOverlays[i].Opacity" :step="0.1" @input="e => handleChange()" />
                </div>

                <div class="labelContainer">
                    <div class="label">
                        {{ colorOverlays[i].label }} Cor
                    </div>
                    <div class="value">
                        {{ data.info.ColorOverlays[i].Color1 }} | {{ getOverlayColorCount() }}
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" :min="0" :max="getOverlayColorCount()" v-model.number="data.info.ColorOverlays[i].Color1" :step="1" @input="e => handleChange()" />
                </div>

                <template v-if="colorOverlays[i].color2 !== undefined">
                    <div class="labelContainer">
                        <div class="label">
                            {{ colorOverlays[i].label }} Cor 2
                        </div>
                        <div class="value">
                            {{ data.info.ColorOverlays[i].Color2 }} | {{ getOverlayColorCount() }}
                        </div>
                    </div>
                    <div class="inputHolder">
                        <input type="range" :min="0" :max="getOverlayColorCount()" v-model.number="data.info.ColorOverlays[i].Color2" :step="1" @input="e => handleChange()" />
                    </div>
                </template>
            </div>
        </div>
    `
});