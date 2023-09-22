Vue.config.devtools = true;
Vue.prototype.window = window;

const app = new Vue({
  el: '#app',
  data() {
    return {
      data: {
        sexo: -1,
        itensCarrinho: [],
        tattoos: [],
        tattooId: -1,
        zoneId: 0,
        message: '',
        zones: [
          {
            id: 'ZONE_HEAD',
            label: 'Cabeça',
          },
          {
            id: 'ZONE_TORSO',
            label: 'Tórax',
          },
          {
            id: 'ZONE_LEFT_ARM',
            label: 'Braço Esquerdo',
          },
          {
            id: 'ZONE_RIGHT_ARM',
            label: 'Braço Direito',
          },
          {
            id: 'ZONE_LEFT_LEG',
            label: 'Perna Esquerda',
          },
          {
            id: 'ZONE_RIGHT_LEG',
            label: 'Perna Direita',
          },
        ]
      },
    };
  },
  methods: {
    setData(_sexo) {
      this.data.sexo = _sexo;
      const zone = this.data.zones[this.data.zoneId];
      if (this.data.sexo == 0)
        this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.female);
      else
        this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.male);
    },
    adicionar() {
      const tattoo = this.data.tattoos[this.data.tattooId];
      if (tattoo == null)
        return;

      this.data.itensCarrinho.push({
        Name: tattoo.name,
        Collection: tattoo.collection,
        Overlay: this.data.sexo == 0 ? tattoo.female : tattoo.male
      });
    },
    removerItem(i) {
      this.data.itensCarrinho.splice(i, 1);
    },
    cancel() {
      if ('alt' in window)
        alt.emit('character:Cancel');
    },
    sync() {
      const tattoo = this.data.tattoos[this.data.tattooId];
      alt.emit('character:Sync', {
        Collection: tattoo.collection,
        Overlay: this.data.sexo == 0 ? tattoo.female : tattoo.male
      });
    },
    confirm() {
      if ('alt' in window)
        alt.emit('character:Done', this.data.itensCarrinho);
    },
    closeMessage() {
      this.data.message = '';
    },
    verCompleto() {
      alt.emit('character:Everything', this.data.itensCarrinho);
    },
    decrementParameter(parameter, min, max, incrementValue) {
      this.data[parameter] -= incrementValue;

      if (this.data[parameter] < min)
        this.data[parameter] = max;

      if (parameter == 'zoneId') {
        const zone = this.data.zones[this.data.zoneId];
        if (this.data.sexo == 0)
          this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.female);
        else
          this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.male);
        this.data.tattooId = -1;
      } else if (parameter == 'tattooId') {
        this.sync();
      }
    },
    incrementParameter(parameter, min, max, incrementValue) {
      this.data[parameter] += incrementValue;

      if (this.data[parameter] > max)
        this.data[parameter] = min;

      if (parameter == 'zoneId') {
        const zone = this.data.zones[this.data.zoneId];
        if (this.data.sexo == 0)
          this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.female);
        else
          this.data.tattoos = tattoos.filter(x => x.zone == zone.id && x.male);
        this.data.tattooId = -1;
      } else if (parameter == 'tattooId') {
        this.sync();
      }
    }
  },
  mounted() {
    if ('alt' in window) {
      alt.on('character:SetData', this.setData);
      alt.on('character:ShowMessage', (message) => {
        this.data.message = message;
      });
    }
  }
});