Vue.component('tab-hair', {
    props: ['data'],
    methods: {
        getHairCount() {
            if (this.data.sex === 0) {
                return femaleHair.length - 1;
            }

            return maleHair.length - 1;
        },
        getColorCount() {
            return hairColors.length - 1;
        },
        getFacialCount() {
            return facialHair.length - 1;
        },
        getEyebrowsCount() {
            return eyebrowNames.length - 1;
        },
        setParameter(parameter, value) {
            this.data[parameter] = value;
            this.$root.$emit('updateCharacter');
        },
        decrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] -= incrementValue;

            if (this.data[parameter] < min)
                this.data[parameter] = max;

            if (parameter == 'hair') {
                if (this.data[parameter] == 24 && this.data.sex == 0)
                    this.data[parameter] -= incrementValue;
                else if (this.data[parameter] == 23 && this.data.sex == 1)
                    this.data[parameter] -= incrementValue;
            }

            this.$root.$emit('updateCharacter');
        },
        incrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] += incrementValue;

            if (this.data[parameter] > max)
                this.data[parameter] = min;

            if (parameter == 'hair') {
                if (this.data[parameter] == 24 && this.data.sex == 0)
                    this.data[parameter] += incrementValue;
                else if (this.data[parameter] == 23 && this.data.sex == 1)
                    this.data[parameter] += incrementValue;
            }

            this.$root.$emit('updateCharacter');
        },
        handleChange(e, parameter, index) {
            const value = parseFloat(e.target.value);
            this.data.colorOverlays[index][parameter] = value;
            this.$root.$emit('updateCharacter');
        }
    },
    template: `
        <div class="options">
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cabelo
                    </div>
                    <div class="value">
                        {{ data.hair }} | {{ getHairCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('hair', 0, getHairCount(), 1)">&#8249;</button>
                    <span> {{ data.hair }}</span>
                    <button class="arrowRight" @click="incrementParameter('hair', 0, getHairCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cor do Cabelo
                    </div>
                    <div class="value">
                        {{ data.hairColor1 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('hairColor1', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.hairColor1 }}</span>
                    <button class="arrowRight" @click="incrementParameter('hairColor1', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Luzes do Cabelo
                    </div>
                    <div class="value">
                        {{ data.hairColor2 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('hairColor2', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.hairColor2 }}</span>
                    <button class="arrowRight" @click="incrementParameter('hairColor2', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>

            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Estilo da Sobrancelha
                    </div>
                    <div class="value">
                        {{ data.eyebrows }} | {{ getEyebrowsCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('eyebrows', 0, getEyebrowsCount(), 1)">&#8249;</button>
                    <span> {{ data.eyebrows }}</span>
                    <button class="arrowRight" @click="incrementParameter('eyebrows', 0, getEyebrowsCount(), 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cor da Sobrancelha
                    </div>
                    <div class="value">
                        {{ data.eyebrowsColor1 }} | {{ getColorCount() }}
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('eyebrowsColor1', 0, getColorCount(), 1)">&#8249;</button>
                    <span> {{ data.eyebrowsColor1 }}</span>
                    <button class="arrowRight" @click="incrementParameter('eyebrowsColor1', 0, getColorCount(), 1)">&#8250;</button>
                </div>
            </div>

            <template v-if="data.sex !== 0">
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.facialHair }} | {{ getFacialCount() }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('facialHair', 0, getFacialCount(), 1)">&#8249;</button>
                        <span> {{ data.facialHair }}</span>
                        <button class="arrowRight" @click="incrementParameter('facialHair', 0, getFacialCount(), 1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Visibilidade do Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.facialHairOpacity.toFixed(1) }} | {{ 1.0 }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('facialHairOpacity', 0, 1, 0.1)">&#8249;</button>
                        <span> {{ data.facialHairOpacity.toFixed(1) }} </span>
                        <button class="arrowRight" @click="incrementParameter('facialHairOpacity', 0, 1, 0.1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Cor do Pelo Facial
                        </div>
                        <div class="value">
                            {{ data.facialHairColor1 }} | {{ getColorCount() }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('facialHairColor1', 0, getColorCount(), 1)">&#8249;</button>
                        <span>{{ data.facialHairColor1 }}</span>
                        <button class="arrowRight" @click="incrementParameter('facialHairColor1', 0, getColorCount(), 1)">&#8250;</button>
                    </div>
                </div>
            </template>


        </div>
    `
});
