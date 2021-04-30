Vue.component('tab-accessories', {
    props: ['data'],
    methods: {
        isActive(parameter, value) {
            if (this.data[parameter] === value) {
                return { active: true };
            }

            return { active: false };
        },
        setParameter(parameter, value) {
            if (parameter == 'acessorioTipo') {
                switch(this.data.tiposAcessorios[value].id) {
                    case 0:
                        this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories0Female : accessories0Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 1:
                        this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories1Female : accessories1Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 2:
                        this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories2Female : accessories2Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 6:
                        this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories6Female : accessories6Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 7:
                        this.data.acessoriosModelos = (this.data.sexo == 0 ? accessories7Female : accessories7Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                }

                let val = this.data.acessorios.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposAcessorios[this.data.acessorioTipo].id);
                this.data.acessorioModelo = this.data.acessoriosModelos.findIndex(x => x.drawable == this.data.acessorios[val].Drawable);
                this.data.acessorioTextura = this.data.acessorios[val].Texture;
            } else if (parameter == 'acessorioModelo') {
                let val = this.data.acessorios.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposAcessorios[this.data.acessorioTipo].id);
                let modelo = this.data.acessoriosModelos[value];
                if (modelo != undefined)
                    this.data.acessorios[val].Drawable = modelo.drawable;
                this.data.acessorios[val].Texture = 0;
                this.data.acessorioTextura = 0;
            } else if (parameter == 'acessorioTextura') {
                let val = this.data.acessorios.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposAcessorios[this.data.acessorioTipo].id);
                this.data.acessorios[val].Texture = value;
            }

            this.$root.$emit('updateCharacter');
        },
        decrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] -= incrementValue;

            if (this.data[parameter] < min) {
                this.data[parameter] = max;
            }

            this.setParameter(parameter, this.data[parameter]);
        },
        incrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] += incrementValue;

            if (this.data[parameter] > max) {
                this.data[parameter] = min;
            }

            this.setParameter(parameter, this.data[parameter]);
        }
    },
    template: `
        <div class="options">
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Tipo
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('acessorioTipo', 0, data.tiposAcessorios.length - 1, 1)">&#8249;</button>
                    <span> {{ data.tiposAcessorios[data.acessorioTipo].label }}</span>
                    <button class="arrowRight" @click="incrementParameter('acessorioTipo', 0, data.tiposAcessorios.length - 1, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Modelo
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('acessorioModelo', 0, data.acessoriosModelos.length - 1, 1)">&#8249;</button>
                    <span> {{ data.acessoriosModelos[data.acessorioModelo] != undefined ? data.acessoriosModelos[data.acessorioModelo].drawable : 0 }}</span>
                    <button class="arrowRight" @click="incrementParameter('acessorioModelo', 0, data.acessoriosModelos.length - 1, 1)">&#8250;</button>
                </div>
            </div>
            <div class="option">
                <div class="labelContainer">
                    <div class="label">
                        Textura
                    </div>
                </div>
                <div class="controls">
                    <button class="arrowLeft" @click="decrementParameter('acessorioTextura', 0, 30, 1)">&#8249;</button>
                    <span> {{ data.acessorioTextura }}</span>
                    <button class="arrowRight" @click="incrementParameter('acessorioTextura', 0, 30, 1)">&#8250;</button>
                </div>
            </div>
        </div>
    `
});