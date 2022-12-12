import * as alt from 'alt-client';
import * as natives from 'natives';

class ObjectStreamer {
    constructor() {
        this.objects = {};
    }

    async addObject(entityId, pos, data) {
        this.removeObject(+entityId);
        this.clearObject(+entityId);

        const handle = new alt.Object(data.model, pos, data.rotation, false, data.dynamic);
        const obj = { handle, entityId, data, pos };
        this.objects[entityId] = obj;
        this.setLodDistance( +entityId, data.lodDistance );
        this.setTextureVariation( +entityId, data.textureVariation );
        this.setVisible( +entityId, data.visible );
        this.setOnFire( +entityId, data.onFire );
        this.setFrozen( +entityId, data.freeze );
        this.setLightColor( +entityId, data.lightColor );
        this.setCollision( +entityId, data.collision );
    }

    getObject( entityId ) {
        if(this.objects.hasOwnProperty(entityId))
            return this.objects[entityId];
        else
            return null;
    }

    async restoreObject( entityId ) {
        if (this.objects.hasOwnProperty(entityId)) {
            const obj = this.objects[entityId];
            const data = obj.data;
            const pos = obj.pos;
            const handle = new alt.Object(data.model, pos, data.rotation, false, data.dynamic);
            this.objects[entityId].handle = handle;
            this.setLodDistance( +entityId, data.lodDistance );
            this.setTextureVariation( +entityId, data.textureVariation );
            this.setVisible( +entityId, data.visible ); 
            this.setOnFire( +entityId, data.onFire );
            this.setFrozen( +entityId, data.freeze );
            this.setLightColor( +entityId, data.lightColor );
            this.setCollision( +entityId, data.collision );
        }
    }

    removeObject(entityId) {
        if (this.objects.hasOwnProperty(entityId)) {
            this.objects[entityId].handle.destroy();
            this.objects[entityId].handle = null;
        }
    }

    clearObject(entityId) {
        if (this.objects.hasOwnProperty(entityId)){
            delete this.objects[entityId];
        }
    }

    clearAllObject() {
        this.objects= {};
    }

    setRotation( entityId, rot ) {
        if(this.objects.hasOwnProperty(entityId)){
            this.objects[entityId].handle.rot = rot;
        }
    }

    setPosition( entityId, pos ) {
      if(this.objects.hasOwnProperty(entityId)){
        this.objects[entityId].handle.pos = pos;
        this.objects[entityId].pos = pos;
      }
    }

    setLodDistance( entityId, lodDistance ) {
      if(this.objects.hasOwnProperty(entityId) && lodDistance !== null){
        this.objects[entityId].handle.lodDistance = lodDistance;
      }
    }

    setTextureVariation( entityId, textureVariation = null ) {
      if(this.objects.hasOwnProperty(entityId) && textureVariation !== null){
        this.objects[entityId].handle.textureVariation = textureVariation;
      }
    }

    setVisible( entityId, visible ) {
      if(this.objects.hasOwnProperty(entityId) && visible !== null){
        this.objects[entityId].handle.visible = visible;
      }
    }

    setOnFire( entityId, onFire = null ) {
      if(this.objects.hasOwnProperty(entityId) && onFire !== null){
        if( !!onFire )
        {
            this.objects[entityId].fireHandle = natives.startScriptFire( this.objects[entityId].pos.x, this.objects[entityId].pos.y, this.objects[entityId].pos.z, 1, true );
        }
        else
        {
            if( this.objects[entityId].fireHandle !== null )
            {
                natives.removeScriptFire( this.objects[entityId].fireHandle );
                this.objects[entityId].fireHandle = null;
            }
        }

        this.objects[entityId].onFire = !!onFire;
      }
    }

    setFrozen( entityId, frozen ) {
      if(this.objects.hasOwnProperty(entityId) && frozen !== null){
        this.objects[entityId].handle.setPositionFrozen(frozen);
      }
    }

    setLightColor( entityId, lightColor = {r:0,g:0,b:0} ) {
      if(this.objects.hasOwnProperty(entityId) && lightColor !== null){
        natives.setPropLightColor( this.objects[entityId].handle, true, lightColor.r, lightColor.g, lightColor.b );
      }
    }

    setCollision( entityId, collision ) {
      if(this.objects.hasOwnProperty(entityId) && collision !== null){
        this.objects[entityId].handle.toggleCollision(collision, collision);
      }
    }
}

export const objStreamer = new ObjectStreamer();

alt.on("resourceStop", () => {
    objStreamer.clearAllObject();
});