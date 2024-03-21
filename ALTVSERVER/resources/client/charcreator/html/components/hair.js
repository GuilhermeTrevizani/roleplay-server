Vue.component('tab-hair', {
  props: ['data'],
  methods: {
    getHairCount() {
      if (this.data.sexo === 0)
        return femaleHairs.length - 1;

      return maleHairs.length - 1;
    },
    getHairOverlayCount() {
      if (this.data.sexo === 0)
        return femaleHairOverlays.length - 1;

      return maleHairOverlays.length - 1;
    },
    getColorCount() {
      return 59;
    },
    getFacialCount() {
      return 30;
    },
    getEyebrowsCount() {
      return 34;
    },
    setParameter(parameter, value) {
      this.data.info[parameter] = value;
      this.$root.$emit('updateCharacter');
    },
    decrementParameter(parameter, min, max, incrementValue) {
      if (parameter == 'cabelo') {
        this.data.cabelo -= incrementValue;

        if (this.data.cabelo < min)
          this.data.cabelo = max;

        if (this.data.sexo === 0) {
          const hair = femaleHairs[this.data.cabelo];
          this.data.info.Hair = hair.drawable;
          this.data.info.HairDLC = hair.dlc;
        } else if (this.data.sexo === 1) {
          const hair = maleHairs[this.data.cabelo];
          this.data.info.Hair = hair.drawable;
          this.data.info.HairDLC = hair.dlc;
        }
      } else if (parameter == 'cabeloDetalhe') {
        this.data.cabeloDetalhe -= incrementValue;

        if (this.data.cabeloDetalhe < min)
          this.data.cabeloDetalhe = max;

        if (this.data.sexo === 0) {
          const hair = femaleHairOverlays[this.data.cabeloDetalhe];
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        } else if (this.data.sexo === 1) {
          const hair = maleHairOverlays[this.data.cabeloDetalhe];
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        }
      } else {
        this.data.info[parameter] -= incrementValue;

        if (this.data.info[parameter] < min)
          this.data.info[parameter] = max;
      }

      this.$root.$emit('updateCharacter');
    },
    incrementParameter(parameter, min, max, incrementValue) {
      if (parameter == 'cabelo') {
        this.data.cabelo += incrementValue;

        if (this.data.cabelo > max)
          this.data.cabelo = min;

        if (this.data.sexo === 0) {
          const hair = femaleHairs[this.data.cabelo];
          this.data.info.Hair = hair.drawable;
          this.data.info.HairDLC = hair.dlc;
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        } else if (this.data.sexo === 1) {
          const hair = maleHairs[this.data.cabelo];
          this.data.info.Hair = hair.drawable;
          this.data.info.HairDLC = hair.dlc;
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        }
      } else if (parameter == 'cabeloDetalhe') {
        this.data.cabeloDetalhe += incrementValue;

        if (this.data.cabeloDetalhe > max)
          this.data.cabeloDetalhe = min;

        if (this.data.sexo === 0) {
          const hair = femaleHairOverlays[this.data.cabeloDetalhe];
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        } else if (this.data.sexo === 1) {
          const hair = maleHairOverlays[this.data.cabeloDetalhe];
          this.data.info.HairCollection = hair.collection;
          this.data.info.HairOverlay = hair.overlay;
        }
      } else {
        this.data.info[parameter] += incrementValue;

        if (this.data.info[parameter] > max)
          this.data.info[parameter] = min;
      }

      this.$root.$emit('updateCharacter');
    },
    handleChange(e, parameter, index) {
      const value = parseFloat(e.target.value);
      this.data.info.ColorOverlays[index][parameter] = value;
      this.$root.$emit('updateCharacter');
    },
  },
  template: `
        <div class="options">
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cabelo
                    </div>
                    <div class="value">
                        {{ data.cabelo }} | {{ getHairCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('cabelo', 0, getHairCount(), 1)">&#8249;</button>
                    <span> {{ data.cabelo }}</span>
                    <button class="arrowRight" @click="incrementParameter('cabelo', 0, getHairCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Detalhe do Cabelo
                    </div>
                    <div class="value">
                        {{ data.cabeloDetalhe }} | {{ getHairOverlayCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('cabeloDetalhe', 0, getHairOverlayCount(), 1)">&#8249;</button>
                    <span> {{ data.cabeloDetalhe }}</span>
                    <button class="arrowRight" @click="incrementParameter('cabeloDetalhe', 0, getHairOverlayCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cor do Cabelo
                    </div>
                    <div class="value">
                        {{ data.info.HairColor1 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('HairColor1', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.info.HairColor1 }}</span>
                    <button class="arrowRight" @click="incrementParameter('HairColor1', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Luzes do Cabelo
                    </div>
                    <div class="value">
                        {{ data.info.HairColor2 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('HairColor2', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.info.HairColor2 }}</span>
                    <button class="arrowRight" @click="incrementParameter('HairColor2', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>

            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Estilo da Sobrancelha
                    </div>
                    <div class="value">
                        {{ data.info.Eyebrows }} | {{ getEyebrowsCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('Eyebrows', 0, getEyebrowsCount(), 1)">&#8249;</button>
                    <span> {{ data.info.Eyebrows }}</span>
                    <button class="arrowRight" @click="incrementParameter('Eyebrows', 0, getEyebrowsCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cor da Sobrancelha
                    </div>
                    <div class="value">
                        {{ data.info.EyebrowsColor1 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('EyebrowsColor1', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.info.EyebrowsColor1 }}</span>
                    <button class="arrowRight" @click="incrementParameter('EyebrowsColor1', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>

            <template v-if="data.sexo !== 0">
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.info.FacialHair }} | {{ getFacialCount() }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('FacialHair', 0, getFacialCount(), 1)">&#8249;</button>
                        <span> {{ data.info.FacialHair }}</span>
                        <button class="arrowRight" @click="incrementParameter('FacialHair', 0, getFacialCount(), 1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Visibilidade do Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.info.FacialHairOpacity.toFixed(1) }} | {{ 1.0 }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('FacialHairOpacity', 0, 1, 0.1)">&#8249;</button>
                        <span> {{ data.info.FacialHairOpacity.toFixed(1) }} </span>
                        <button class="arrowRight" @click="incrementParameter('FacialHairOpacity', 0, 1, 0.1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Cor do Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.info.FacialHairColor1 }} | {{ getColorCount() }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('FacialHairColor1', 0, getColorCount(), 1)">&#8249;</button>
                        <span>{{ data.info.FacialHairColor1 }}</span>
                        <button class="arrowRight" @click="incrementParameter('FacialHairColor1', 0, getColorCount(), 1)">&#8250;</button>
                    </div>
                </div>
            </template>
        </div>
    `
});