Vue.component('tab-done', {
    props: ['data'],
    methods: {
        saveCharacter() {
            if ('alt' in window) 
                alt.emit('character:Done', this.data.roupas, this.data.acessorios, this.data.roupa);
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
                <template v-if="data.tipo != 0">
                    <button class="full danger" @click="cancel" style="margin-bottom: 12px">Cancelar</button>
                </template>
            </div>
        </div>
    `
});