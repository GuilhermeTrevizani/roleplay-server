Vue.config.devtools = true;
Vue.prototype.window = window;

const app = new Vue({
    el: '#app',
    data() {
        return {
            selection: 0,
            data: {
                info: {},
                barbearia: false,
                cabelo: 0,
                sexo: 0,
                cabeloDetalhe: 0,
            },
            navOptions: ['Sex', 'Structure', 'Hair', 'Overlays', 'Decor', 'Done']
        };
    },
    computed: {
        isInactiveNext() {
            if (this.selection >= this.navOptions.length - 1) 
                return { inactive: true };

            return { inactive: false };
        },
        isInactiveBack() {
            if (this.selection <= 0) 
                return { inactive: true };

            return { inactive: false };
        },
        getTabComponent: function() {
            return `tab-${this.navOptions[this.selection].toLowerCase()}`;
        }
    },
    methods: {
        setData(_sexo, _info, _barbearia) {
            if (_barbearia)
                this.navOptions = ['Hair', 'Decor', 'Done'];

            this.data.sexo = _sexo;
            this.data.info = _info;
            this.data.barbearia = _barbearia;
            
            this.data.cabelo = (this.sexo === 0 ? femaleHairs : maleHairs).findIndex(x => x.drawable == this.data.info.Hair && x.dlc == this.data.info.HairDLC);
            if (this.data.cabelo === -1)
                this.data.cabelo = 0;
            
            this.data.cabeloDetalhe = (this.sexo === 0 ? femaleHairOverlays : maleHairOverlays).findIndex(x => x.collection == this.data.info.HairCollection && x.overlay == this.data.info.HairOverlay);
            if (this.data.cabeloDetalhe === -1)
                this.data.cabeloDetalhe = 0;
        },
        goNext() {
            if (this.selection >= this.navOptions.length - 1)
                return;

            this.selection += 1;
        },
        goBack() {
            if (this.selection <= 0)
                return;

            this.selection -= 1;
        },
        updateCharacter() {
            if ('alt' in window)
                alt.emit('character:Sync', this.data.info);
        },
        resetSelection() {
            this.selection = 0;
        }
    },
    mounted() {
        this.$root.$on('updateCharacter', this.updateCharacter);
        this.$root.$on('resetSelection', this.resetSelection);

        if ('alt' in window)
            alt.on('character:SetData', this.setData);
    }
});