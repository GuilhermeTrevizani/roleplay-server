Vue.component('tab-overlays', {
  props: ['data'],
  methods: {
    handleChange(e, parameter, index) {
      const value = parseFloat(e.target.value);
      this.data.info.OpacityOverlays[index][parameter] = value;
      this.$root.$emit('updateCharacter');
    },
    getOverlayColorCount() {
      return 63;
    }
  },
  template: `
        <div class="options">
            <div v-for="(name, i) in opacityOverlays" :key="i" class="option">
                <div class="labelContainer">
                    <div class="label">
                        {{ opacityOverlays[i].label }}
                    </div>
                    <div class="value">
                        {{ data.info.OpacityOverlays[i].Value }} | {{ opacityOverlays[i].max }}
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" :min="opacityOverlays[i].min" :max="opacityOverlays[i].max" v-model.number="data.info.OpacityOverlays[i].Value" :step="opacityOverlays[i].increment" @input="e => handleChange(e, 'Value', i)" />
                </div>
                <div class="labelContainer">
                    <div class="label">
                        Opacidade
                    </div>
                    <div class="value">
                        {{ data.info.OpacityOverlays[i].Opacity.toFixed(1) }} | 1.0
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" :min="0" :max="1" v-model.number="data.info.OpacityOverlays[i].Opacity" :step="0.1" @input="e => handleChange(e, 'Opacity', i)" />
                </div>
            </div>
        </div>
    `
});