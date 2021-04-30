Vue.config.devtools = true;
Vue.prototype.window = window;

const app = new Vue({
    el: '#app',
    data() {
        return {
            selection: 0,
            barbearia: 0,
            data: {
                sex: 0,
                faceFather: 33,
                faceMother: 45,
                skinFather: 45,
                skinMother: 45,
                faceMix: 0.5,
                skinMix: 0.5,
                structure: new Array(20).fill(0),
                hair: 11,
                hairColor1: 5,
                hairColor2: 2,
                hairOverlay: '',
                facialHair: 29,
                facialHairColor1: 62,
                facialHairOpacity: 0,
                eyebrows: 0,
                eyebrowsOpacity: 1,
                eyebrowsColor1: 0,
                eyes: 0,
                opacityOverlays: [],
                colorOverlays: []
            },
            navOptions: ['Sex', 'Structure', 'Hair', 'Overlays', 'Decor', 'Done']
        };
    },
    computed: {
        isInactiveNext() {
            if (this.selection >= this.navOptions.length - 1) {
                return { inactive: true };
            }

            return { inactive: false };
        },
        isInactiveBack() {
            if (this.selection <= 0) {
                return { inactive: true };
            }

            return { inactive: false };
        },
        getTabComponent: function() {
            return `tab-${this.navOptions[this.selection].toLowerCase()}`;
        }
    },
    methods: {
        setData(oldData, _barbearia) {
            if (!oldData) {
                this.updateCharacter();
                return;
            }

            if (_barbearia == 1)
                this.navOptions = ['Hair', 'Done'];
            else if (_barbearia == 2)
                this.navOptions = ['Decor', 'Done'];

            this.data = oldData;
            this.barbearia = _barbearia;
            this.updateCharacter();
        },
        goNext() {
            if (this.selection >= this.navOptions.length - 1) {
                return;
            }

            this.selection += 1;
        },
        goBack() {
            if (this.selection <= 0) {
                return;
            }

            this.selection -= 1;
        },
        updateCharacter() {
            const isFemale = this.data.sex === 0;
            this.data.hairOverlay = isFemale ? femaleHairOverlays[this.data.hair] : maleHairOverlays[this.data.hair];

            if (isFemale) {
                this.data.facialHair = 30;
                this.data.facialHairOpacity = 0;
            }

            // Update Floats
            this.data.skinMix = parseFloat(this.data.skinMix);
            this.data.faceMix = parseFloat(this.data.faceMix);

            if ('alt' in window)
                alt.emit('character:Sync', this.data);
        },
        resetSelection() {
            this.selection = 0;
        }
    },
    mounted() {
        this.$root.$on('updateCharacter', this.updateCharacter);
        this.$root.$on('resetSelection', this.resetSelection);

        opacityOverlays.forEach(overlay => {
            const overlayData = { ...overlay };
            overlayData.value = 0;
            delete overlayData.key;
            delete overlayData.max;
            delete overlayData.min;
            delete overlayData.label;
            delete overlayData.increment;

            this.data.opacityOverlays.push(overlayData);
        });

        colorOverlays.forEach(overlay => {
            const overlayData = { ...overlay };
            overlayData.value = 0;
            delete overlayData.key;
            delete overlayData.max;
            delete overlayData.min;
            delete overlayData.label;
            delete overlayData.increment;

            this.data.colorOverlays.push(overlayData);
        });

        if ('alt' in window)
            alt.on('character:SetData', this.setData);
    }
});
