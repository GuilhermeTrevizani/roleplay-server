Vue.config.devtools = true;
Vue.prototype.window = window;

const app = new Vue({
    el: '#app',
    data() {
        return {
            selection: 0,
            data: {
                roupa: 1,
                slots: 0,
                sexo: 0,
                tipo: 0,
                tipoFaccao: 0,
                roupas: [],
                acessorios: [],
                tiposRoupas: [{ id: 1, label: 'Máscara'},{ id: 3, label: 'Torso'},{ id: 4, label: 'Calça'},{ id: 5, label: 'Mochilas'},{ id: 6, label: 'Sapatos'},{ id: 7, label: 'Complementos'},{ id: 8, label: 'Camisas'},{ id: 9, label: 'Coletes'},{ id: 10, label: 'Bordados'},{ id: 11, label: 'Jaquetas'}],
                tiposAcessorios: [{ id: 0, label: 'Chapéus'},{ id: 1, label: 'Óculos'},{ id: 2, label: 'Orelhas'},{ id: 6, label: 'Relógios'},{ id: 7, label: 'Braceletes'}],
                roupaTipo: 0,
                roupasModelos: [],
                roupaModelo: 0,
                roupaTextura: 0,
                acessorioTipo: 0,
                acessoriosModelos: [],
                acessorioModelo: 0,
                acessorioTextura: 0,
            },
            navOptions: ['Clothes', 'Accessories', 'Done']
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
        setData(_roupas, _acessorios, _sexo, _slots, _roupa, _tipo, _tipoFaccao) {
            if (_tipo == 3)
                this.navOptions = ['Clothes', 'Done'];

            if (_tipoFaccao != 1)
                this.data.tiposRoupas = this.data.tiposRoupas.filter(x => x.id != 9);

            this.data.slots = _slots;
            this.data.tipoFaccao = _tipoFaccao;
            this.data.tipo = _tipo;
            this.data.sexo = _sexo;
            this.data.roupa = _roupa;
            this.data.roupas = _roupas;
            this.data.acessorios = _acessorios;

            this.data.roupasModelos = clothes1.filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
            let val = this.data.roupas.find(x => x.ID == this.data.roupa && x.Slot == 1);
            this.data.roupaModelo = this.data.roupasModelos.findIndex(x => x.drawable == val.Drawable);
            this.data.roupaTextura = val.Texture;
            
            this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories0Female : accessories0Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
            let valAc = this.data.acessorios.find(x => x.ID == this.data.roupa && x.Slot == 0);
            this.data.acessorioModelo = this.data.acessoriosModelos.findIndex(x => x.drawable == valAc.Drawable);
            this.data.acessorioTextura = valAc.Texture;

            this.updateCharacter();
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
                alt.emit('character:SyncClothes', this.data.roupas.filter(x => x.ID == this.data.roupa), this.data.acessorios.filter(x => x.ID == this.data.roupa));
        },
        log(txt) {
            if ('alt' in window)
                alt.emit('log', txt);
        },
    },
    mounted() {
        this.$root.$on('updateCharacter', this.updateCharacter);
        this.$root.$on('log', this.log);

        if ('alt' in window)
            alt.on('character:SetData', this.setData);
    }
});
