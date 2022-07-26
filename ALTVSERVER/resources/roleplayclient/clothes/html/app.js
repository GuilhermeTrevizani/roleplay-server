Vue.config.devtools = true;
Vue.prototype.window = window;

function getJSON(url) {
    return new Promise(resolve => {
        var request = new XMLHttpRequest();
        request.open('GET', url);
        request.responseType = 'json';
        request.onload = function() {
            return resolve(request.response);
        };
        request.send();
    });
}

const BASE_PATH = '../../resources/json/';

let clothes1Male;
getJSON(`${BASE_PATH}clothes1male.json`).then((clothes) => {
    clothes1Male = clothes;
});

let clothes1Female;
getJSON(`${BASE_PATH}clothes1female.json`).then((clothes) => {
    clothes1Female = clothes;
});

let clothes3Male;
getJSON(`${BASE_PATH}clothes3male.json`).then((clothes) => {
    clothes3Male = clothes;
});

let clothes3Female;
getJSON(`${BASE_PATH}clothes3female.json`).then((clothes) => {
    clothes3Female = clothes;
});

let clothes4Male;
getJSON(`${BASE_PATH}clothes4male.json`).then((clothes) => {
    clothes4Male = clothes;
});

let clothes4Female;
getJSON(`${BASE_PATH}clothes4female.json`).then((clothes) => {
    clothes4Female = clothes;
});

let clothes5Male;
getJSON(`${BASE_PATH}clothes5male.json`).then((clothes) => {
    clothes5Male = clothes;
});

let clothes5Female;
getJSON(`${BASE_PATH}clothes5female.json`).then((clothes) => {
    clothes5Female = clothes;
});

let clothes6Male;
getJSON(`${BASE_PATH}clothes6male.json`).then((clothes) => {
    clothes6Male = clothes;
});

let clothes6Female;
getJSON(`${BASE_PATH}clothes6female.json`).then((clothes) => {
    clothes6Female = clothes;
});

let clothes7Male;
getJSON(`${BASE_PATH}clothes7male.json`).then((clothes) => {
    clothes7Male = clothes;
});

let clothes7Female;
getJSON(`${BASE_PATH}clothes7female.json`).then((clothes) => {
    clothes7Female = clothes;
});

let clothes8Male;
getJSON(`${BASE_PATH}clothes8male.json`).then((clothes) => {
    clothes8Male = clothes;
});

let clothes8Female;
getJSON(`${BASE_PATH}clothes8female.json`).then((clothes) => {
    clothes8Female = clothes;
});

let clothes9Male;
getJSON(`${BASE_PATH}clothes9male.json`).then((clothes) => {
    clothes9Male = clothes;
});

let clothes9Female;
getJSON(`${BASE_PATH}clothes9female.json`).then((clothes) => {
    clothes9Female = clothes;
});

let clothes10Male;
getJSON(`${BASE_PATH}clothes10male.json`).then((clothes) => {
    clothes10Male = clothes;
});

let clothes10Female;
getJSON(`${BASE_PATH}clothes10female.json`).then((clothes) => {
    clothes10Female = clothes;
});

let clothes11Male;
getJSON(`${BASE_PATH}clothes11male.json`).then((clothes) => {
    clothes11Male = clothes;
});

let clothes11Female;
getJSON(`${BASE_PATH}clothes11female.json`).then((clothes) => {
    clothes11Female = clothes;
});

let accessories0Male;
getJSON(`${BASE_PATH}accessories0male.json`).then((clothes) => {
    accessories0Male = clothes;
});

let accessories0Female;
getJSON(`${BASE_PATH}accessories0female.json`).then((clothes) => {
    accessories0Female = clothes;
});

let accessories1Male;
getJSON(`${BASE_PATH}accessories1male.json`).then((clothes) => {
    accessories1Male = clothes;
});

let accessories1Female;
getJSON(`${BASE_PATH}accessories1female.json`).then((clothes) => {
    accessories1Female = clothes;
});

let accessories2Male;
getJSON(`${BASE_PATH}accessories2male.json`).then((clothes) => {
    accessories2Male = clothes;
});

let accessories2Female;
getJSON(`${BASE_PATH}accessories2female.json`).then((clothes) => {
    accessories2Female = clothes;
});

let accessories6Male;
getJSON(`${BASE_PATH}accessories6male.json`).then((clothes) => {
    accessories6Male = clothes;
});

let accessories6Female;
getJSON(`${BASE_PATH}accessories6female.json`).then((clothes) => {
    accessories6Female = clothes;
});

let accessories7Male;
getJSON(`${BASE_PATH}accessories7male.json`).then((clothes) => {
    accessories7Male = clothes;
});

let accessories7Female;
getJSON(`${BASE_PATH}accessories7female.json`).then((clothes) => {
    accessories7Female = clothes;
});

const app = new Vue({
    el: '#app',
    data() {
        return {
            data: {
                sexo: 0,
                tipo: 0,
                tipoFaccao: 0,
                itens: [],
                itensCarrinho: [],
                componentes: [
                    { id: 1, component: 1, label: 'Máscara'},
                    { id: 2, component: 3, label: 'Torso'},
                    { id: 3, component: 4, label: 'Calça'},
                    { id: 4, component: 5, label: 'Mochila'},
                    { id: 5, component: 6, label: 'Sapato'},
                    { id: 6, component: 7, label: 'Extra'},
                    { id: 7, component: 8, label: 'Camisa'},
                    { id: 8, component: 9, label: 'Colete'},
                    { id: 9, component: 10, label: 'Bordado'},
                    { id: 10, component: 11, label: 'Jaqueta'},
                    { id: 11, component: 0, label: 'Chapéu'},
                    { id: 12, component: 1, label: 'Óculos'},
                    { id: 13, component: 2, label: 'Orelha'},
                    { id: 14, component: 6, label: 'Relógio'},
                    { id: 15, component: 7, label: 'Bracelete'}
                ],
                componente: 0,
                modelos: [],
                modelo: 0,
                textura: 0,
                message: '',
            },
        };
    },
    methods: {
        setData(_sexo, _tipo, _tipoFaccao) {
            this.data.tipoFaccao = _tipoFaccao;
            this.data.tipo = _tipo;
            this.data.sexo = _sexo;

            this.setParameter('componente', 0);
        },
        adicionar() {
            if (this.data.tipo == 0) {
                alt.emit('character:Done', this.data.itens.filter(x => x.Drawable != -1 && x.Cloth), 
                    this.data.itens.filter(x => x.Drawable != -1 && !x.Cloth));
            } else {
                const tipo = this.data.componentes[this.data.componente];
                let obj = {... this.data.itens.find(x => x.ID == tipo.id)};
                if (obj.Drawable == -1)
                    return;

                obj.Label = tipo.label;
                this.data.itensCarrinho.push(obj);
            }
        },
        removerItem(i) {
            this.data.itensCarrinho.splice(i, 1);
        },
        cancel() {
            if ('alt' in window) 
                alt.emit('character:Cancel');
        },
        confirm() {
            if ('alt' in window)
                alt.emit('character:Done', this.data.itensCarrinho.filter(x => x.Cloth), this.data.itensCarrinho.filter(x => !x.Cloth));
        },
        closeMessage() {
            this.data.message = '';
        },
        filterClothes(clothes) {
            return clothes.filter(x => (this.data.tipo != 2 && x.tipoFaccao == 0) 
                || (this.data.tipo == 2 && (x.tipoFaccao == this.data.tipoFaccao 
                    || (x.tipoFaccao == -2 && (this.data.tipoFaccao == 1 || this.data.tipoFaccao == 2))))
                || x.tipoFaccao == -1);
        },
        setParameter(parameter, value) {
            const component = this.data.componentes[this.data.componente];
            let item = this.data.itens.find(x => x.ID == component.id);

            if (parameter == 'componente') {
                switch(component.id) {
                    case 1:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes1Female : clothes1Male);
                        break;
                    case 2:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes3Female : clothes3Male);
                        break;
                    case 3:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes4Female : clothes4Male);
                        break;
                    case 4:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes5Female : clothes5Male);
                        break;
                    case 5:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes6Female : clothes6Male);
                        break;
                    case 6:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes7Female : clothes7Male);
                        break;
                    case 7:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes8Female : clothes8Male);
                        break;
                    case 8:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes9Female : clothes9Male);
                        break;
                    case 9:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes10Female : clothes10Male);
                        break;
                    case 10:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? clothes11Female : clothes11Male);
                        break;
                    case 11:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? accessories0Female : accessories0Male);
                        break;
                    case 12:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? accessories1Female : accessories1Male);
                        break;
                    case 13:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? accessories2Female : accessories2Male);
                        break;
                    case 14:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? accessories6Female : accessories6Male);
                        break;
                    case 15:
                        this.data.modelos = this.filterClothes(this.data.sexo == 0 ? accessories7Female : accessories7Male);
                        break;
                }
    
                if (!item) {
                    const modelo = this.data.modelos[0];
                    item = {
                        ID: component.id,
                        Cloth: component.id <= 10,
                        Component: component.component,
                        Drawable: modelo?.drawable ?? 0,
                        Texture: 0,
                        DLC: modelo?.dlc,
                    };

                    this.data.itens.push(item);
                    this.data.modelo = 0;
                    this.data.textura = 0;
                } else {
                    this.data.modelo = this.data.modelos.findIndex(x => x.drawable == item.Drawable);
                    this.data.textura = item.Texture;
                }
                return;
            } else if (parameter == 'modelo') {
                const modelo = this.data.modelos[value];
                if (modelo) {
                    item.Drawable = modelo.drawable;
                    item.DLC = modelo.dlc;

                    if (modelo.maxTexture === undefined) {
                        if ('alt' in window) 
                            alt.emit(item.Cloth ? 'character:GetClothMaxTexture' : 'character:GetPropMaxTexture',
                                item.Component, 
                                item.Drawable
                            );
                    }
                }

                this.data.textura = item.Texture = 0;
            } else if (parameter == 'textura') {
                item.Texture = value;
            }
            
            if ('alt' in window)  {
                if (item.Cloth)
                    alt.emit('character:SetClothes', item.Component, item.Drawable, item.Texture, item.DLC);
                else
                    alt.emit('character:SetProps', item.Component, item.Drawable, item.Texture, item.DLC);
            }
        },
        decrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] -= incrementValue;

            if (this.data[parameter] < min)
                this.data[parameter] = max;

            this.setParameter(parameter, this.data[parameter]);
        },
        incrementParameter(parameter, min, max, incrementValue) {
            this.data[parameter] += incrementValue;

            if (this.data[parameter] > max)
                this.data[parameter] = min;

            this.setParameter(parameter, this.data[parameter]);
        }
    },
    mounted() {
        if ('alt' in window) {
            alt.on('character:SetData', this.setData);
            alt.on('character:ShowMessage', (message) => {
                this.data.message = message;
            });
            alt.on('character:SetMaxTexture', (maxTexture) => {
                const modelo = this.data.modelos[this.data.modelo];
                modelo.maxTexture = maxTexture - 1;
            });
        }
    }
});