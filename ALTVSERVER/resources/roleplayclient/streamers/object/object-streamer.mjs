import * as alt from 'alt-client';
import * as natives from 'natives';

async function loadModel(modelText) {
    let model = alt.hash(modelText);
    return new Promise(resolve => {

        if (!natives.isModelValid(model))
            return resolve(false);

        if (natives.hasModelLoaded(model))
            return resolve(true);

            natives.requestModel(model);

        let interval = alt.setInterval(() => {
                if (natives.hasModelLoaded(model)) {
                    resolve(true);
                    alt.clearInterval(interval);
                }
            },
            5);
    });
}

class ObjectStreamer {
    constructor( ) {
        this.objects = {};
    }

    async addObject( entityId, model, entityType, pos, rot, lodDistance, textureVariation, dynamic, visible, onFire, frozen, lightColor, collision  ) {
        // clear the object incase it still exists.
        this.removeObject( +entityId );
        this.clearObject( +entityId );

        loadModel(model).then(() =>
        {
            let handle = natives.createObjectNoOffset(alt.hash(model), pos.x, pos.y, pos.z, false, false, dynamic );
            let obj = { handle: handle, entityId: entityId, model: model, entityType: entityType, position: pos, frozen: frozen };
            this.objects[entityId] = obj;
            this.setRotation( +entityId, rot );
            //this.setLodDistance( obj, lodDistance );
            this.setTextureVariation( +entityId, textureVariation );
            this.setDynamic( +entityId, dynamic );
            //this.setVisible( obj, visible );
            this.setOnFire( +entityId, onFire );
            this.setFrozen( +entityId, frozen );
            this.setLightColor( +entityId, lightColor );
            this.setCollision( +entityId, collision );
        });
    }

    getObject( entityId, entityType ) {
      if(this.objects.hasOwnProperty(entityId)){
        return this.objects[entityId];
      }else{
        return null;
      }
    }

    async restoreObject( entityId, entityType ) {
      if(this.objects.hasOwnProperty(entityId)){
        let obj = this.objects[entityId];
        loadModel(obj.model).then(() =>
        {
            this.objects[entityId].handle = natives.createObject(alt.hash(obj.model), obj.position.x, obj.position.y, obj.position.z, false, false, false );
            this.setRotation( +entityId, obj.rotation );
            //this.setLodDistance( obj, obj.lodDistance );
            this.setTextureVariation( +entityId, obj.textureVariation );
            this.setDynamic( +entityId, obj.dynamic );
            //this.setVisible( obj, obj.visible );
            this.setOnFire( +entityId, obj.onFire );
            this.setFrozen( +entityId, obj.frozen );
            this.setLightColor( +entityId, obj.lightColor );
            this.setCollision( +entityId, obj.collision );
        });
      }
    }

    removeObject( entityId, entityType ) {
      if(this.objects.hasOwnProperty(entityId)){
        natives.deleteObject( this.objects[entityId].handle );
        this.objects[entityId].handle = null;
      }
    }

    clearObject( entityId, entityType ) {
      if(this.objects.hasOwnProperty(entityId)){
        delete this.objects[entityId];
      }
    }

    clearAllObject() {
        this.objects= {};
    }

    setRotation( entityId, rot ) {
      if(this.objects.hasOwnProperty(entityId)){
        natives.setEntityRotation( this.objects[entityId].handle, rot.x, rot.y, rot.z, 0, true );
        this.objects[entityId].rotation = rot;
      }
    }
    setVelocity( entityId, vel ) {
      if(this.objects.hasOwnProperty(entityId)){
        natives.setEntityVelocity( this.objects[entityId].handle, vel.x, vel.y, vel.z);
        this.objects[entityId].velocity = vel;
      }
    }
    slideToPosition( entityId, pos, time ) {
        let count = 0;
        natives.slideObject(this.objects[entityId].handle, pos.x, pos.y, pos.z, 8, 8, 8, true);
        /*
        var slide = alt.setInterval(() =>
        {
            alt.log("slideInterval:count:" + count);
            count++;
            if(count >= 10) alt.clearInterval(slide);
            var objectPos = native.getEntityCoords(objet, false);
            var vel = {x,y,z};
            vel.x = (pos.x - objectPos.x) / 3;
            vel.y = (pos.y - objectPos.y) / 3;
            vel.z = (pos.z - objectPos.z) / 3;
		    natives.setEntityVelocity( obj.handle, vel.x, vel.y, vel.z);
        }, time / 10);
        */
    }

    setPosition( entityId, pos ) {
      if(this.objects.hasOwnProperty(entityId)){
        natives.setEntityCoordsNoOffset( this.objects[entityId].handle, pos.x, pos.y, pos.z, true, true, true );
        this.objects[entityId].position = pos;
      }
    }

    async setModel( entityId, model ) {
      if(this.objects.hasOwnProperty(entityId)){
        this.objects[entityId].model = model;
      }
    }

    setLodDistance( entityId, lodDistance ) {
      if(this.objects.hasOwnProperty(entityId) && lodDistance !== null){
        natives.setEntityLodDist( this.objects[entityId].handle, lodDistance );
        this.objects[entityId].lodDistance = lodDistance;
      }
    }

    setTextureVariation( entityId, textureVariation = null ) {
      if(this.objects.hasOwnProperty(entityId) && textureVariation !== null){
        natives.setObjectTextureVariation( this.objects[entityId].handle, textureVariation );
        this.objects[entityId].textureVariation = textureVariation;
      }
    }

    setDynamic( entityId, dynamic ) {
      if(this.objects.hasOwnProperty(entityId) && dynamic !== null){
        natives.setEntityDynamic( this.objects[entityId].handle, !!dynamic );
        this.objects[entityId].dynamic = !!dynamic;
      }
    }

    setVisible( entityId, visible ) {
      if(this.objects.hasOwnProperty(entityId) && visible !== null){
        natives.setEntityVisible( this.objects[entityId].handle, !!visible, false );
        this.objects[entityId].visible = !!visible;
      }
    }

    setOnFire( entityId, onFire = null ) {
      if(this.objects.hasOwnProperty(entityId) && onFire !== null){
        if( !!onFire )
        {
            this.objects[entityId].fireHandle = natives.startScriptFire( this.objects[entityId].position.x, this.objects[entityId].position.y, this.objects[entityId].position.z, 1, true );
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
        natives.freezeEntityPosition( this.objects[entityId].handle, frozen );
        this.objects[entityId].frozen = frozen;
      }
    }

    setLightColor( entityId, lightColor = {r:0,g:0,b:0} ) {
      if(this.objects.hasOwnProperty(entityId) && lightColor !== null){
        //natives.setObjectLightColor( this.objects[entityId].handle, true, lightColor.r, lightColor.g, lightColor.b );
        this.objects[entityId].lightColor = lightColor;
      }
    }

    setCollision( entityId, collision ) {
      if(this.objects.hasOwnProperty(entityId) && collision !== null){
        natives.setEntityCollision( this.objects[entityId].handle, collision, collision);
        this.objects[entityId].collision = !!collision;
      }
    }
}

export const objStreamer = new ObjectStreamer();

alt.on( "resourceStop", ( ) => {
    objStreamer.clearAllObject();
} );