Vue.component('tab-clothes', {
    props: ['data'],
    methods: {
        isActive(parameter, value) {
            if (this.data[parameter] === value) {
                return { active: true };
            }

            return { active: false };
        },
        setParameter(parameter, value) {
            if (parameter == 'roupaTipo') {
                switch(this.data.tiposRoupas[value].id) {
                    case 1:
                        this.data.roupasModelos = clothes1.filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 3:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes3Female : clothes3Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 4:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes4Female : clothes4Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 5:
                        this.data.roupasModelos = clothes5.filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 6:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes6Female : clothes6Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 7:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes7Female : clothes7Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 8:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes8Female : clothes8Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 9:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes9Female : clothes9Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 10:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes10Female : clothes10Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                    case 11:
                        this.data.roupasModelos = (this.data.sexo == 0 ? clothes11Female : clothes11Male).filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) || (this.data.tipo == 2 && x.tipoFaccao == this.data.tipoFaccao) || x.tipoFaccao == -1);
                        break;
                }

                let val = this.data.roupas.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposRoupas[this.data.roupaTipo].id);
                this.data.roupaModelo = this.data.roupasModelos.findIndex(x => x.drawable == this.data.roupas[val].Drawable);
                this.data.roupaTextura = this.data.roupas[val].Texture;
            } else if (parameter == 'roupaModelo') {
                let val = this.data.roupas.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposRoupas[this.data.roupaTipo].id);
                let modelo = this.data.roupasModelos[value];
                if (modelo != undefined)
                    this.data.roupas[val].Drawable = modelo.drawable;
                this.data.roupas[val].Texture = 0;
                this.data.roupaTextura = 0;
            } else if (parameter == 'roupaTextura') {
                let val = this.data.roupas.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposRoupas[this.data.roupaTipo].id);
                this.data.roupas[val].Texture = value;
            } else if (parameter == 'roupa') {
                this.data.roupaTipo = 0;
                this.data.acessorioTipo = 0;

                let val = this.data.roupas.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposRoupas[this.data.roupaTipo].id);
                this.data.roupaModelo = this.data.roupasModelos.findIndex(x => x.drawable == this.data.roupas[val].Drawable);
                this.data.roupaTextura = this.data.roupas[val].Texture;

                val = this.data.acessorios.findIndex(x => x.ID == this.data.roupa && x.Slot == this.data.tiposAcessorios[this.data.acessorioTipo].id);
                this.data.acessorioModelo = this.data.acessoriosModelos.findIndex(x => x.drawable == this.data.acessorios[val].Drawable);
                this.data.acessorioTextura = this.data.acessorios[val].Texture;
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
            <template v-if="data.tipo != 0">
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Slot
                        </div>
                        <div class="value">
                            {{ data.roupa }} | {{ data.slots }}
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('roupa', 1, data.slots, 1)">&#8249;</button>
                        <span> {{ data.roupa }} </span>
                        <button class="arrowRight" @click="incrementParameter('roupa', 1, data.slots, 1)">&#8250;</button>
                    </div>
                </div>
            </template>
            <template v-if="data.tipo != 3">
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Tipo
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('roupaTipo', 0, data.tiposRoupas.length - 1, 1)">&#8249;</button>
                        <span> {{ data.tiposRoupas[data.roupaTipo].label }}</span>
                        <button class="arrowRight" @click="incrementParameter('roupaTipo', 0, data.tiposRoupas.length - 1, 1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Modelo
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('roupaModelo', 0, data.roupasModelos.length - 1, 1)">&#8249;</button>
                        <span> {{ data.roupasModelos[data.roupaModelo] != undefined ? data.roupasModelos[data.roupaModelo].drawable : 0 }}</span>
                        <button class="arrowRight" @click="incrementParameter('roupaModelo', 0, data.roupasModelos.length - 1, 1)">&#8250;</button>
                    </div>
                </div>
                <div class="option">
                    <div class="labelContainer">
                        <div class="label">
                            Textura
                        </div>
                    </div>
                    <div class="controls">
                        <button class="arrowLeft" @click="decrementParameter('roupaTextura', 0, 20, 1)">&#8249;</button>
                        <span> {{ data.roupaTextura }}</span>
                        <button class="arrowRight" @click="incrementParameter('roupaTextura', 0, 20, 1)">&#8250;</button>
                    </div>
                </div>
            </template>
        </div>
    `
});