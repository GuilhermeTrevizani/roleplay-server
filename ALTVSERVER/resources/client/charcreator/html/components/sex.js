Vue.component('tab-sex', {
    props: ['data'],
    methods: {
        isActive(parameter, value) {
            if (this.data.info[parameter] === value)
                return { active: true };

            return { active: false };
        },
        setParameter(parameter, value) {
            if (isNaN(value)) 
                this.data.info[parameter] = value;
            else
                this.data.info[parameter] = parseFloat(value);

            this.$root.$emit('updateCharacter');
        },
        decrementParameter(parameter, min, max, incrementValue) {
            this.data.info[parameter] -= incrementValue;

            if (this.data.info[parameter] < min) 
                this.data.info[parameter] = max;

            this.$root.$emit('updateCharacter');
        },
        incrementParameter(parameter, min, max, incrementValue) {
            this.data.info[parameter] += incrementValue;

            if (this.data.info[parameter] > max) 
                this.data.info[parameter] = min;

            this.$root.$emit('updateCharacter');
        }
    },
    watch: {
        'data.info.FaceMix': function(newVal, oldVal) {
            this.$root.$emit('updateCharacter');
        },
        'data.info.SkinMix': function(newVal, oldVal) {
            this.$root.$emit('updateCharacter');
        }
    },
    template: `
        <div class="options">
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Face do Pai
                    </div>
                    <div class="value">
                        {{ data.info.FaceFather }} | 45
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('FaceFather', 0, 45, 1)">&#8249;</button>
                    <span> {{ data.info.FaceFather }}</span>
                    <button class="arrowRight" @click="incrementParameter('FaceFather', 0, 45, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Pele do Pai
                    </div>
                    <div class="value">
                        {{ data.info.SkinFather }} | 45
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('SkinFather', 0, 45, 1)">&#8249;</button>
                    <span> {{ data.info.SkinFather }}</span>
                    <button class="arrowRight" @click="incrementParameter('SkinFather', 0, 45, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Face da Mãe
                    </div>
                    <div class="value">
                        {{ data.info.FaceMother }} | 45
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('FaceMother', 0, 45, 1)">&#8249;</button>
                    <span> {{ data.info.FaceMother }}</span>
                    <button class="arrowRight" @click="incrementParameter('FaceMother', 0, 45, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Pele da Mãe
                    </div>
                    <div class="value">
                        {{ data.info.SkinMother }} | 45
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('SkinMother', 0, 45, 1)">&#8249;</button>
                    <span> {{ data.info.SkinMother }}</span>
                    <button class="arrowRight" @click="incrementParameter('SkinMother', 0, 45, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                       Mistura das Faces
                    </div>
                    <div class="value">
                        {{ parseFloat(data.info.FaceMix).toFixed(1) }} | 1.0
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" min="0" max="1" step="0.1" v-model.number="data.info.FaceMix"/>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                       Mistura das Peles
                    </div>
                    <div class="value">
                        {{ parseFloat(data.info.SkinMix).toFixed(1) }} | 1.0
                    </div>
                </div>
                <div class="inputHolder">
                    <input type="range" min="0.0" max="1.0" step="0.1" v-model.number="data.info.SkinMix"/>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Cor dos Olhos
                    </div>
                    <div class="value">
                        {{ data.info.Eyes }} | 7
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('Eyes', 0, 7, 1)">&#8249;</button>
                    <span> {{ data.info.Eyes }} </span>
                    <button class="arrowRight" @click="incrementParameter('Eyes', 0, 7, 1)">&#8250;</button>
                </div>
            </div>
        </div>
    `
});