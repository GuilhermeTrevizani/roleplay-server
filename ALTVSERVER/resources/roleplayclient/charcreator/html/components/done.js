Vue.component('tab-done', {
    props: ['data', 'barbearia'],
    methods: {
        saveCharacter() {
            if (this.data.sex === 0) {
                this.data.facialHair = 29;
                this.data.facialHairColor1 = 0;
            }

            if ('alt' in window) 
                alt.emit('character:Done', this.data);
        },
        cancel() {
            if ('alt' in window) 
                alt.emit('character:Cancel');
        },
    },
    template: `
        <div class="options">
            <p>Clique no botão abaixo para confirmar a customização do seu personagem.</p>
            <div class="option">
                <button class="full" @click="saveCharacter" style="margin-bottom: 12px">Confirmar</button>
                <template v-if="barbearia != 0">
                    <button class="full danger" @click="cancel" style="margin-bottom: 12px">Cancelar</button>
                </template>
            </div>
        </div>
    `
});
