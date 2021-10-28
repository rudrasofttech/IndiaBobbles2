var scenery = {
    ground: { height: 50 },
    rubble: { height: 81 },
    launcher: {
        height: 70,
        width: 91,
        alfl: 20, // left position of agni respect to launcher
        ahfg: 30 //Height at which agni should be placed from ground
    },
    bgbuildings: { width: 1423, height: 214 },
    friend: { width: 46, height: 15 },
    enemy: { width: 56, height: 10 },
    milestone: { left: 120, bottom: 20, height: 50, width: 42 },
    lotus: {
        x: 360, height: 113, width: 237,
        layer1: { width: 205, height: 43 },
        layer2: { width: 137, height: 40 },
        layer3: { width: 100, height: 25 }
    },
    bd2: { x: 965, height: 108, width: 125 },
    bd6: { x: 1015, height: 133, width: 67.25 },
    bd7: { X: 1088, height: 35, width: 82 },
    bd8: { x: 180, height: 190, width: 44.75 },
    bd5: { x: 1175, height: 81, width: 98.25 },
    bd1: { x: 675, height: 165, width: 282, lhratio: 0.4848, lwratio: 1 },
    bd10: { x: 0, height: 64, width: 193 },
    bd14: { x: 1280, height: 117, width: 57.25, left: 55 },
    bd3: { x: 230, height: 108, width: 125, left: 5 },
    bd12: { x: 607, height: 204, width: 61.75, left: 10 },
    levelNameStyle: { font: "75px armalite", fill: "#fff", align: "center" },
    commonTextStyle: { font: "20px armalite", fill: "#fff", align: "center" },
    subHeadingTextStyle: { font: "30px armalite", fill: "#fff", align: "center" },
    buttonTextStyle: { font: "40px armalite", fill: "#fff", align: "center" },
    healthWarningStyle: { font: "30px armalite", fill: "#8B0000", align: "center" },
    panickerShoutStyle: { font: "10px arial", fill: "#fff", align: "center" },
    mouseOverTextColor: "#E8C252",
    mouseOutTextColor: "#ffffff",
    noAgniWarningColor: "#8B0000",
    noEnemyWarningColor: "#000000"

};
//bd10 | bd8 > bd3 > lotus > bd12 > bd1 > bd2 | bd6 > bd7 > bd5 > bd14
// Initialize Phaser, and creates a 400x490px game
var game = new Phaser.Game(1366, 768, Phaser.AUTO, 'gameDiv');
var fireRate = 500;
var nextFire = 0;
var sounds, bgsound, agnisound, expsound, expbdsound, exphitsound, clicksound;
//manage and emit particles
var emitter = null;
var raining = false;
TURN_RATE = 5;
SPEED = 350;
SMOKE_LIFETIME = 3000;

var PanickerShouts = {
    One: "help", Two: "my nose", Three: "stupid", Four: "#@!&", Five: "mara", Six: "ui maa",
    GetAny: function () {
        var chosenOne = Math.floor(Math.random() * 5) + 1
        if (chosenOne == 1) {
            return this.One;
        } else if (chosenOne == 2) {
            return this.Two;
        } else if (chosenOne == 3) {
            return this.Three;
        } else if (chosenOne == 4) {
            return this.Four;
        } else if (chosenOne == 5) {
            return this.Five;
        } else if (chosenOne == 6) {
            return this.Five;
        }
    }
}
var MSpeed = {
    VerySlow: 90, Slow: 100, Normal: 130, Fast: 155, VeryFast: 180,
    GetAny: function () {
        var chosenOne = Math.floor(Math.random() * 5) + 1
        if (chosenOne == 1) {
            return this.VerySlow;
        } else if (chosenOne == 2) {
            return this.Slow;
        } else if (chosenOne == 3) {
            return this.Normal;
        } else if (chosenOne == 4) {
            return this.Fast;
        } else if (chosenOne == 5) {
            return this.VeryFast;
        }
    }
};
var initState = {
    preload: function () {
        game.load.image('startscreen', 'images/startscreen.jpg');
        game.load.image('startbtn', 'images/start.png');
        game.load.image('nextbtn', 'images/next.png');
        game.load.image('restartbtn', 'images/restart.png');
        game.load.image('backbtn', 'images/back.png');
        game.load.image('bg1', 'images/bg1.jpg');
        game.load.image('bg2', 'images/bg2.jpg');
        game.load.image('rubble', 'images/rubble.png');
        game.load.image('bgbuildings', 'images/Bgbuildings_Sprite.png');
        game.load.image('milestone', 'images/milestone.png');
        game.load.spritesheet('explode', 'images/explode.png', 128, 128);
        game.load.spritesheet('panicker', 'images/panicker.png', 15, 25);
        game.load.image('fullvolume', 'images/fullvolume.png');
        game.load.image('novolume', 'images/novolume.png');

        game.load.image('ground', 'images/ground.png');
        game.load.image('friend', 'images/friend1.png');
        game.load.image('enemy', 'images/enemy1.png');
        game.load.image('station', 'images/launcher.png');
        game.load.image('spacer', 'images/spacer.png');
        game.load.image('smoke', 'images/smoke-puff.png');
        game.load.image('pointer', 'images/pointer.png');
        game.load.image('popup', 'images/popup.png');
        game.load.spritesheet('rain', 'images/rain.png', 17, 17);
        game.load.spritesheet('lotus', 'images/bd4.png', 237, 113);
        game.load.spritesheet('bd2', 'images/bd2.png', 125, 108);
        game.load.spritesheet('bd6', 'images/bd6.png', 67.25, 133);
        game.load.spritesheet('bd7', 'images/bd7.png', 82, 35);
        game.load.spritesheet('bd8', 'images/bd8.png', 44.75, 190);
        game.load.spritesheet('bd5', 'images/bd5.png', 98.25, 81);
        game.load.spritesheet('bd1', 'images/bd1.png', 282, 165);
        game.load.spritesheet('bd10', 'images/bd10.png', 193, 64);
        game.load.spritesheet('bd14', 'images/bd14.png', 57.25, 117);
        game.load.spritesheet('bd3', 'images/bd3.png', 125, 108);
        game.load.spritesheet('bd12', 'images/bd12.png', 61.75, 204);

        //load music files
        game.load.audio('bg', ['sounds/bg.mp3', 'sounds/bg.ogg']);
        game.load.audio('agni', ['sounds/agni.mp3', 'sounds/agni.ogg']);
        game.load.audio('exp', ['sounds/exp.mp3', 'sounds/exp.ogg']);
        game.load.audio('expbd', ['sounds/exp-bd.mp3', 'sounds/exp-bd.ogg']);
        game.load.audio('exphit', ['sounds/exp-hit.mp3', 'sounds/exp-hit.ogg']);
        game.load.audio('click', ['sounds/click.mp3', 'sounds/click.ogg']);
    },
    create: function () {
        if (localStorage.volumestatus == undefined) {
            localStorage.volumestatus = true;
        }

        this.ss = this.game.add.sprite(game.world.centerX, game.world.centerY, "startscreen");
        this.ss.anchor.setTo(0.5);

        this.startbtn = this.game.add.button(game.world.centerX + 200, game.world.centerY + 50, "startbtn");
        this.startbtn.anchor.setTo(0.5);
        clicksound = new Phaser.Sound(game, "click", 1, false);
        this.startbtn.onInputDown.add(function () {

            startClickSound();
            game.state.start('stage');
        }, this);
    },
};

var stageOptionState = {
    preload: function () { },
    create: function () {

        var s = new SceneBuilder(this);
        game.input.mouse.capture = true;
        if (localStorage.stageCleared == undefined) {
            localStorage.stageCleared = 0;
        }
        if (localStorage.stageCleared >= 0) {
            var tutorial = game.add.text(game.world.centerX, 100, "Training", scenery.levelNameStyle);
            tutorial.anchor.set(0.5);
            tutorial.inputEnabled = true;

            tutorial.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            tutorial.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            tutorial.events.onInputDown.add(function (item) {
                startClickSound();
                game.state.start('tutorial');
            }, this);
        }

        if (localStorage.stageCleared >= 1) {
            var level1 = game.add.text(game.world.centerX, 200, "First Blood", scenery.levelNameStyle);
            level1.anchor.set(0.5);
            level1.inputEnabled = true;

            level1.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            level1.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOutTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            level1.events.onInputDown.add(function (item) {
                startClickSound();
                game.state.start('level1');
            }, this);
        }

        if (localStorage.stageCleared >= 2) {
            var level2 = game.add.text(game.world.centerX, 300, "More Will Come", scenery.levelNameStyle);
            level2.anchor.set(0.5);
            level2.inputEnabled = true;

            level2.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            level2.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOutTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            level2.events.onInputDown.add(function (item) {
                startClickSound();
                game.state.start('level2');
            }, this);
        }

        if (localStorage.stageCleared >= 3) {
            var level3 = game.add.text(game.world.centerX, 400, "No End In Sight", scenery.levelNameStyle);
            level3.anchor.set(0.5);
            level3.inputEnabled = true;

            level3.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            level3.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOutTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            level3.events.onInputDown.add(function (item) {
                startClickSound();
                mainState.maxEnemy = 30;
                mainState.maxShots = 30;
                mainState.hits = 0;
                mainState.maxHits = 5;
                mainState.stageIndex = 3;
                mainState.enemySpeed = MSpeed.Normal;
                mainState.successMessage = "Good Job!\nMove To Next Stage";
                mainState.failureMessage = "You don't \n have it in you";
                mainState.levelIntroMessage = "Hope you are ready for more mayhem, enemy has just launched 30 missiles.\n If 5 missiles hit the city you will loose.";
                game.state.start('level3');
            }, this);
        }

        if (localStorage.stageCleared >= 4) {
            var level4 = game.add.text(game.world.centerX, 500, "Only Few Bullets", scenery.levelNameStyle);
            level4.anchor.set(0.5);
            level4.inputEnabled = true;

            level4.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            level4.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOutTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            level4.events.onInputDown.add(function (item) {
                startClickSound();
                mainState.maxEnemy = 30;
                mainState.maxShots = 24;
                mainState.stageIndex = 4;
                mainState.hits = 0;
                mainState.maxHits = 5;
                mainState.enemySpeed = MSpeed.Normal;
                mainState.successMessage = "Good Job!\nWar is still on.";
                mainState.failureMessage = "We trusted you :(";
                mainState.levelIntroMessage = "We wish we would have stocked piled more Agni missiles,\n you have to defend the city with 24 missiles against \n 30 enemies. If 5 missiles hit the city you will loose. ";
                game.state.start('level4');
            }, this);
        }

        if (localStorage.stageCleared >= 5) {
            var level5 = game.add.text(game.world.centerX, 600, "Do or Die", scenery.levelNameStyle);
            level5.anchor.set(0.5);
            level5.inputEnabled = true;

            level5.events.onInputOver.add(function (item) {
                item.fill = scenery.mouseOverTextColor;
                this.game.canvas.style.cursor = "pointer";
            }, this);

            level5.events.onInputOut.add(function (item) {
                item.fill = scenery.mouseOutTextColor;
                this.game.canvas.style.cursor = "default";
            }, this);

            level5.events.onInputDown.add(function (item) {
                startClickSound();
                mainState.maxEnemy = 50;
                mainState.maxShots = 45;
                mainState.stageIndex = 5;
                mainState.hits = 0;
                mainState.maxHits = 5;
                mainState.enemySpeed = MSpeed.Fast;
                mainState.successMessage = "WOW!\n You did it.\nYou saved the city.";
                mainState.failureMessage = "Millions Perished\n You gave up easily.";
                mainState.levelIntroMessage = "It's now or never, enemy is frustrated and has began his final assault with 50 missiles,\n we have managed to produce 45 Agni missiles to counter their attack.\n Our future depends on you now. \n If 5 missiles hit the city you will loose.";
                game.state.start('level5');
            }, this);
        }
    },
    update: function () { }
};

var tutorialState = {
    allowFire: false,
    shotsFired: 0,
    create: function () {
        this.shotsFired = 0;
        //set scene for this state
        var s = new SceneBuilder(this);

        this.enemy1 = new EnemyMissile(game, 200, -100, 300, 150, MSpeed.VerySlow);
        //once the first enemy reaches destination stop it and display message
        this.enemy1.destinationSignal.add(function () {
            this.help.setText("Touch the white dot");
            this.help.alpha = 1;
            this.pointer.alpha = 1;

            //stoping enemy
            this.enemy1.body.velocity.x = 0;
            this.enemy1.body.velocity.y = 0;
            this.enemy1.smokeEmitter.on = false;
            //position pointer to show where to click
            this.pointer.x = this.enemy1.x + 90;
            this.pointer.y = this.enemy1.y + 200;
        }, this);
        //if first enemy is destroyed by friend fire another enemy missile
        this.enemy1.destroySignal.add(function () {
            this.help.setText("Good now try on your own, here comes another one.");
            this.help.alpha = 1;
            this.createBomb();
            this.allowFire = true;
            game.add.tween(this.help).to({ alpha: 0 }, 2000, Phaser.Easing.Linear.None, true, 300);
        }, this);
        this.bombs.add(this.enemy1);

        this.pointer.events.onInputDown.add(function (item) {
            //allow friend fire again
            this.allowFire = true;
            if (this.enemy1 != undefined) {
                this.enemy1.smokeEmitter.on = true;
                this.enemy1.target = { x: 600, y: game.world.height };
                this.enemy1.body.velocity.x = 10;
                this.enemy1.body.velocity.y = 10;
                this.enemy1.fire();
                item.alpha = 0;
                item.inputEnabled = false;
            }

        }, this);
        this.startstage.events.onInputDown.add(function (item) {
            var tween = game.add.tween(item).to({ alpha: 0 }, 600, Phaser.Easing.Linear.None, true, 300);
            tween.onComplete.add(function (sprite, tween) {
                sprite.destroy();
                tutorialState.allowFire = true;
                this.help.alpha = 1;

                //  Being mp3 files these take time to decode, so we can't play them instantly
                //  Using setDecodedCallback we can be notified when they're ALL ready for use.
                //  The audio files could decode in ANY order, we can never be sure which it'll be.
                game.sound.setDecodedCallback(sounds, startSound, this);
            }, this);
        }, this);


    },
    update: function () {
        if (game.input.activePointer.isDown && this.allowFire) {
            this.fire(game.input.activePointer.x, game.input.activePointer.y);
        }

        game.physics.arcade.collide(this.bombs, this.ground, this.bombCollisionWithGround, null, this);
        game.physics.arcade.overlap(this.bombs, this.explosions, this.bombCollisionWithExplosion, null, this);
        game.physics.arcade.collide(this.bombs, this.friends, this.bombCollisionWithFriend, null, this);

        this.cleanUp();
    },
    fire: function (x, y) {
        if (game.time.now > nextFire) {
            var startpoint = { x: this.mstation.x + scenery.launcher.alfl, y: game.world.height - (scenery.ground.height + scenery.launcher.ahfg) };
            nextFire = game.time.now + fireRate;

            var f = new Friend(this.game, startpoint.x, startpoint.y, x, y, tutorialState);
            f.explosionSignal.add(function () {
                //blast friend and show explosion
                this.showExplosion(f.x, f.y);

                //if its the second shot fire first enemy
                if (this.shotsFired == 1) {
                    this.enemy1.fire();

                }
            }, this);
            this.shotsFired += 1;
            this.friends.add(f);
            if (this.shotsFired == 1) {
                //forbid friend fire for now
                this.allowFire = false;
            }
            this.help.alpha = 0;

        }
    },
    showExplosion: function (x, y) {
        var xpl = this.explosions.create(x, y, 'explode');
        xpl.anchor.setTo(0.5, 0.5);
        startFriendExp();
        xplanim = xpl.animations.add('xplode');
        xpl.animations.play('xplode', 65, false);
        xplanim.onComplete.add(function (sprite, animation) { sprite.destroy(); }, this);
    },
    bombCollisionWithGround: function (ground, bomb) {
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        //if bomb manages to collide with ground allow player one more shot
        this.createBomb();
        this.help.setText("Oops, focus and try again!");
        this.help.alpha = 1;
        game.add.tween(this.help).to({ alpha: 0 }, 2000, Phaser.Easing.Linear.None, true, 300);
    },
    bombCollisionWithExplosion: function (bomb, explosion) {
        bomb.blast();

        //see if first two shots are already fired then show move next popup
        if (this.shotsFired > 2) {
            this.allowFire = false;
            game.time.events.add(Phaser.Timer.SECOND, function () { this.showPopup(); }, this);

        }
    },
    bombCollisionWithFriend: function (bomb, friend) {
        //show a collision at bombs coordinates and
        //kill bomd and friend as well
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        friend.blast();
        //see if first two shots are already fired then show move next popup
        if (this.shotsFired > 2) {
            this.allowFire = false;
            game.time.events.add(Phaser.Timer.SECOND, function () { this.showPopup(); }, this);
        }
    },
    createBomb: function () {
        var x = Math.floor((Math.random() * game.world.width) + 1);
        var x2 = Math.floor((Math.random() * (game.world.width)) + 1);

        var missile = new EnemyMissile(this.game, x, -200, x2, game.world.height, MSpeed.Slow);

        this.bombs.add(missile);
        missile.fire();
        return missile;
    },
    showPopup: function () {
        var p = new Popup(this, "Good Job!\n Go save the city.", "Next", function (item) {
            stopStartSound();
            if (parseInt(localStorage.stageCleared, 10) < 1) {
                localStorage.stageCleared = 1;
            }

            game.state.start('stage');
        });
    },
    cleanUp: function () {
        this.bombs.forEachDead(function (item) {
            item.destroy();
        });
        this.friends.forEachDead(function (item) {
            item.destroy();
        });
        this.explosions.forEachDead(function (item) {
            item.destroy();
        });
    }
};

var level1State = {
    allowFire: false,
    shotsFired: 0,
    enemyFired: 0,
    enemyDead: 0,
    maxShots: 6,
    maxEnemy: 6,
    maxHits: 1,
    hits: 0,
    create: function () {

        this.allowFire = false;
        this.shotsFired = 0;
        this.enemyFired = 0;
        this.enemyDead = 0;
        this.hits = 0;
        var s = new SceneBuilder(this);

        this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";
        this.enemyremaininglbl.text = (this.maxEnemy - this.enemyFired) + " Missiles";
        this.help.setText("We have intercepted 6 incoming enemy missiles, each targeting the Lotus Temple.\n Protect it at all cost. A single hit and you will loose.")
        this.help.alpha = 1;
        this.help.y = game.world.centerY - 200;
        this.startstage.events.onInputDown.add(function (item) {
            var tween = game.add.tween(item).to({ alpha: 0 }, 600, Phaser.Easing.Linear.None, true, 300);
            tween.onComplete.add(function (sprite, tween) {
                startSound();
                sprite.destroy();
                this.help.destroy();
                level1State.allowFire = true;
                var m = this.createBomb(MSpeed.VeryFast);
                m.destroySignal.add(function () {

                    this.createBomb(MSpeed.Normal);
                    this.createBomb(MSpeed.Slow);

                    game.time.events.add(5000, function () { this.createBomb(MSpeed.Normal); }, this);
                    game.time.events.add(10000, function () {
                        this.createBomb(MSpeed.Slow);
                        this.createBomb(MSpeed.Fast);
                    }, this);
                }, this);

            }, this);
        }, this);
    },
    update: function () {
        game.physics.arcade.collide(this.bombs, this.ground, this.bombCollisionWithGround, null, this);

        if (game.input.activePointer.isDown && this.allowFire) {
            this.fire(game.input.activePointer.x, game.input.activePointer.y);
        }


        game.physics.arcade.collide(this.bombs, this.friends, this.bombCollisionWithFriend, null, this);
        game.physics.arcade.overlap(this.bombs, this.explosions, this.bombCollisionWithExplosion, null, this);
        game.physics.arcade.collide(this.lotusside3, this.bombs, this.enemyhitlotusside3, null, this);
        game.physics.arcade.collide(this.lotusside2, this.bombs, this.enemyhitlotusside2, null, this);
        game.physics.arcade.collide(this.lotusside1, this.bombs, this.enemyhitlotusside1, null, this);

        this.cleanUp();

        if (this.hits >= this.maxHits) {
            this.allowFire = false;
            game.time.events.add(Phaser.Timer.SECOND * 2, function () { this.showFailurePopup() }, this);
        }
        else if (this.hits < this.maxHits && this.enemyDead >= this.maxEnemy) {
            this.allowFire = false;
            game.time.events.add(Phaser.Timer.SECOND * 2, function () { this.showSuccessPopup(); }, this);
        }
    },
    fire: function (x, y) {
        if (game.time.now > nextFire && this.shotsFired < this.maxShots) {
            var startpoint = { x: this.mstation.x + scenery.launcher.alfl, y: game.world.height - (scenery.ground.height + scenery.launcher.ahfg) };
            nextFire = game.time.now + fireRate;

            var f = new Friend(this.game, startpoint.x, startpoint.y, x, y, tutorialState);
            f.explosionSignal.add(function () {
                //blast friend and show explosion
                this.showExplosion(f.x, f.y);
            }, this);
            this.shotsFired += 1;
            this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";
            if ((this.maxShots - this.shotsFired) <= 0) {
                this.friendlyremaininglbl.stroke = scenery.noAgniWarningColor;
                this.friendlyremaininglbl.strokeThickness = 6;
            }
            this.friends.add(f);

        }
    },
    showExplosion: function (x, y) {
        var xpl = this.explosions.create(x, y, 'explode');
        xpl.anchor.setTo(0.5, 0.5);

        xplanim = xpl.animations.add('xplode');
        xpl.animations.play('xplode', 65, false);
        xplanim.onComplete.add(function (sprite, animation) { sprite.destroy(); }, this);
    },
    bombCollisionWithGround: function (ground, bomb) {
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        this.enemyDead += 1;
    },
    bombCollisionWithExplosion: function (bomb, explosion) {
        bomb.blast();
        this.enemyDead += 1;
    },
    bombCollisionWithFriend: function (bomb, friend) {
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        friend.blast();
        this.enemyDead += 1;
    },
    enemyhitlotusside1: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;

        layer.kill();
        startBdExp();
        this.lotusside2.destroy();
        this.lotusside3.destroy();
        this.lotus.frame = 3;
        this.hits += 1;
        this.showHealth();
        this.showPanickers(layer.x + (layer.width / 2));
    },
    enemyhitlotusside2: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;

        layer.kill();
        startBdExp();
        this.lotusside3.destroy();

        this.lotus.frame = 2;
        this.hits += 1;
        this.showHealth();
        this.showPanickers(layer.x + (layer.width / 2));
    },
    enemyhitlotusside3: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;

        layer.kill();
        startBdExp();
        this.lotus.frame = 1;
        this.hits += 1;
        this.showHealth();
        this.showPanickers(layer.x + (layer.width / 2));
    },
    createBomb: function (s) {
        if (this.enemyFired < this.maxEnemy) {
            var x = Math.floor((Math.random() * game.world.width) + 1);
            var x2 = this.lotus.x + this.lotus.width / 2;
            //Math.floor(Math.random() * (this.enemySpeedRange.max - this.enemySpeedRange.min + 1)) + this.enemySpeedRange.min
            var missile = new EnemyMissile(this.game, x, -200, x2, game.world.height, s);
            this.bombs.add(missile);
            missile.fire();
            this.enemyFired += 1;
            this.updateEnemyLabel();
            return missile;
        }
        return null;
    },
    showSuccessPopup: function () {
        var p = new Popup(this, "Good Job!\nSoon more will come.", "Next", function (item) {
            stopStartSound();
            if (parseInt(localStorage.stageCleared, 10) < 2) {
                localStorage.stageCleared = 2;
            }

            game.state.start('stage');
        });
    },
    showFailurePopup: function () {
        var p = new Popup(this, "You had just one job!", "Back", function (item) {
            stopStartSound();
            game.state.start('stage');
        });
    },
    updateEnemyLabel: function () {
        this.enemyremaininglbl.text = (this.maxEnemy - this.enemyFired) + " Missiles";
        if ((this.maxEnemy - this.enemyFired) <= 0) {
            this.enemyremaininglbl.stroke = scenery.noEnemyWarningColor;
            this.enemyremaininglbl.strokeThickness = 6;
        }
    },
    showHealth: function () {
        if (this.hits > 1) {
            this.healthwarninglbl.text = this.hits + " hits";
        } else {
            this.healthwarninglbl.text = this.hits + " hit";
        }
    },
    cleanUp: function () {
        this.bombs.forEachDead(function (item) {
            item.destroy();
        });
        this.friends.forEachDead(function (item) {
            item.destroy();
        });
        this.explosions.forEachDead(function (item) {
            item.destroy();
        });
        this.panickers.forEachDead(function (item) {
            item.destroy();
        });
    },
    showPanickers: function (x) {
        var no = Math.floor((Math.random() * 5) + 1);
        for (iCount = 0; iCount <= no; iCount++) {

            game.time.events.add(Math.floor((Math.random() * 1000) + 1), function () {
                var p = new Panicker(game, x, game.world.height - scenery.ground.height);
                this.panickers.add(p);
            }, this);

        }
    }
};

var level2State = {
    buildingDestroyed: false,
    allowFire: false,
    shotsFired: 0,
    enemyFired: 0,
    enemyDead: 0,
    maxShots: 10,
    maxEnemy: 10,
    hits: 0,
    maxHits: 3,
    create: function () {

        this.allowFire = false;
        this.shotsFired = 0;
        this.enemyFired = 0;
        this.enemyDead = 0;
        this.hits = 0;
        this.buildingDestroyed = false;
        var s = new SceneBuilder(this);

        this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";
        this.enemyremaininglbl.text = (this.maxEnemy - this.enemyFired) + " Missiles";
        this.help.setText("10 more enemy9 missiles are heading your way targeting few heritage sites and commercial buildings.\n " + this.maxHits + " hits and you will loose.")
        this.help.alpha = 1;
        this.help.y = game.world.centerY - 200;
        this.startstage.events.onInputDown.add(function (item) {
            var tween = game.add.tween(item).to({ alpha: 0 }, 600, Phaser.Easing.Linear.None, true, 300);
            tween.onComplete.add(function (sprite, tween) {
                startSound();
                sprite.destroy();
                this.help.destroy();
                this.allowFire = true;
                var m = this.createBomb(this.lotus.x + this.lotus.width / 2, MSpeed.Fast);
                m.destroySignal.add(function () {
                    this.createBomb(this.bd8.x, MSpeed.Slow);
                    this.createBomb(this.bd8.x, MSpeed.Fast);

                    game.time.events.add(5000, function () { this.createBomb(this.lotus.x + this.lotus.width / 2, MSpeed.Normal); }, this);
                    game.time.events.add(10000, function () {
                        this.createBomb(this.bd8.x, MSpeed.Normal);
                        this.createBomb(this.bd5.x + this.bd5.width / 2, MSpeed.VeryFast);
                    }, this);

                    game.time.events.add(12000, function () { this.createBomb(this.lotus.x + this.lotus.width / 2, MSpeed.VeryFast); }, this);
                    game.time.events.add(13000, function () { this.createBomb(this.bd5.x + this.bd5.width / 2, MSpeed.VeryFast); }, this);

                    game.time.events.add(15000, function () {
                        this.createBomb(this.bd8.x, MSpeed.Normal);
                        this.createBomb(this.bd5.x + this.bd5.width / 2, MSpeed.Fast);
                    }, this);
                }, this);

            }, this);
        }, this);
    },
    update: function () {
        game.physics.arcade.collide(this.bombs, this.ground, this.bombCollisionWithGround, null, this);

        if (game.input.activePointer.isDown && this.allowFire) {
            this.fire(game.input.activePointer.x, game.input.activePointer.y);
        }

        game.physics.arcade.overlap(this.bombs, this.friends, this.bombCollisionWithFriend, null, this);
        game.physics.arcade.overlap(this.bombs, this.explosions, this.bombCollisionWithExplosion, null, this);
        game.physics.arcade.overlap(this.lotusside3, this.bombs, this.enemyhitlotusside3, null, this);
        game.physics.arcade.overlap(this.lotusside2, this.bombs, this.enemyhitlotusside2, null, this);
        game.physics.arcade.overlap(this.lotusside1, this.bombs, this.enemyhitlotusside1, null, this);
        game.physics.arcade.overlap(this.bd2layer, this.bombs, this.enemyhitbd2layer, null, this);
        game.physics.arcade.overlap(this.bd8layer, this.bombs, this.enemyhitbd8layer, null, this);
        game.physics.arcade.overlap(this.bd5layer, this.bombs, this.enemyhitbd5layer, null, this);

        this.cleanUp();

        if (this.hits >= this.maxHits) {
            this.allowFire = false;
            game.time.events.add(2000, function () { this.showFailurePopup() }, this);
        }
        else if (this.hits < this.maxHits && this.enemyDead >= this.maxEnemy) {
            this.allowFire = false;
            game.time.events.add(2000, function () { this.showSuccessPopup(); }, this);
        }
    },
    fire: function (x, y) {
        if (game.time.now > nextFire && this.shotsFired < this.maxShots) {
            var startpoint = { x: this.mstation.x + scenery.launcher.alfl, y: game.world.height - (scenery.ground.height + scenery.launcher.ahfg) };
            nextFire = game.time.now + fireRate;

            var f = new Friend(this.game, startpoint.x, startpoint.y, x, y, tutorialState);
            f.explosionSignal.add(function () {
                //blast friend and show explosion
                this.showExplosion(f.x, f.y);
            }, this);
            this.shotsFired += 1;
            this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";

            if ((this.maxShots - this.shotsFired) <= 0) {
                this.friendlyremaininglbl.stroke = scenery.noAgniWarningColor;
                this.friendlyremaininglbl.strokeThickness = 6;
            }
            this.friends.add(f);

        }
    },
    showExplosion: function (x, y) {
        var xpl = this.explosions.create(x, y, 'explode');
        xpl.anchor.setTo(0.5, 0.5);

        xplanim = xpl.animations.add('xplode');
        xpl.animations.play('xplode', 65, false);
        xplanim.onComplete.add(function (sprite, animation) { sprite.destroy(); }, this);
    },
    bombCollisionWithGround: function (ground, bomb) {
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        this.enemyDead += 1;
    },
    bombCollisionWithExplosion: function (bomb, explosion) {
        bomb.blast();
        this.enemyDead += 1;
    },
    bombCollisionWithFriend: function (bomb, friend) {
        this.showExplosion(bomb.x, bomb.y);
        bomb.blast();
        friend.blast();
        this.enemyDead += 1;
    },
    enemyhitlotusside1: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;

        layer.kill();
        startBdExp();
        this.lotusside2.destroy();
        this.lotusside3.destroy();
        this.lotus.frame = 3;
        this.hits += 1;
        this.showHealth();
        this.buildingDestroyed = true;
    },
    enemyhitlotusside2: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;

        layer.kill();
        this.lotusside3.destroy();
        startBdExp();
        this.lotus.frame = 2;
        this.hits += 1;
        this.showHealth();
        this.showPanickers(layer.x + (layer.width / 2));
    },
    enemyhitlotusside3: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        this.enemyDead += 1;
        layer.kill();
        startBdExp();
        this.lotus.frame = 1;
        this.hits += 1;
        this.showHealth();
        this.showPanickers(layer.x + (layer.width / 2));
    },
    enemyhitbd2layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        startBdExp();
        this.enemyDead += 1;

        if (this.bd2.frame < 3) {
            this.bd2.frame += 1;
        }

        if (this.bd2.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6363;
            layer.height = layer.height * 0.6363;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }

        if (this.bd2.frame == 2) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }
    },
    enemyhitbd8layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        startBdExp();
        this.enemyDead += 1;

        if (this.bd8.frame < 3) {
            this.bd8.frame += 1;
        }
        if (this.bd8.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }
        else if (this.bd8.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6;
            layer.height = layer.height * 0.6;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }

        if (this.bd8.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }
    },
    enemyhitbd5layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        enemy.blast();
        startBdExp();
        this.enemyDead += 1;

        if (this.bd5.frame < 3) {
            this.bd5.frame += 1;
        }
        if (this.bd5.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }
        else if (this.bd5.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6;
            layer.height = layer.height * 0.6;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }

        if (this.bd5.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
            this.showPanickers(layer.x + (layer.width / 2));
        }
    },

    createBomb: function (targetx, s) {
        if (this.enemyFired < this.maxEnemy) {
            var x = Math.floor((Math.random() * game.world.width) + 1);
            //var x2 = this.lotus.x + this.lotus.width / 2;
            //Math.floor(Math.random() * (this.enemySpeedRange.max - this.enemySpeedRange.min + 1)) + this.enemySpeedRange.min
            var missile = new EnemyMissile(this.game, x, -200, targetx, game.world.height, s);
            this.bombs.add(missile);
            missile.fire();
            this.enemyFired += 1;
            this.updateEnemyLabel();
            return missile;
        }
        return null;
    },
    showSuccessPopup: function () {
        var p = new Popup(this, "Good Job!\n You shall pass.", "Next", function (item) {
            stopStartSound();
            if (parseInt(localStorage.stageCleared, 10) < 3) {
                localStorage.stageCleared = 3;
            }
            game.state.start('stage');
        });
    },
    showFailurePopup: function () {
        var p = new Popup(this, "Disappointing!", "Back", function (item) {
            stopStartSound();
            game.state.start('stage');
        });
    },
    updateEnemyLabel: function () {
        this.enemyremaininglbl.text = (this.maxEnemy - this.enemyFired) + " Missiles";
        if ((this.maxEnemy - this.enemyFired) <= 0) {
            this.enemyremaininglbl.stroke = scenery.noEnemyWarningColor;
            this.enemyremaininglbl.strokeThickness = 6;
        }
    },
    showHealth: function () {
        if (this.hits > 1) {
            this.healthwarninglbl.text = this.hits + " hits";
        } else {
            this.healthwarninglbl.text = this.hits + " hit";
        }
    },
    cleanUp: function () {
        this.bombs.forEachDead(function (item) {
            item.destroy();
        });
        this.friends.forEachDead(function (item) {
            item.destroy();
        });
        this.explosions.forEachDead(function (item) {
            item.destroy();
        });
        this.panickers.forEachDead(function (item) {
            item.destroy();
        });
    },
    showPanickers: function (x) {
        var no = Math.floor((Math.random() * 5) + 1);
        for (iCount = 0; iCount <= no; iCount++) {

            game.time.events.add(Math.floor((Math.random() * 1000) + 1), function () {
                var p = new Panicker(game, x, game.world.height - scenery.ground.height);
                this.panickers.add(p);
            }, this);

        }
    }
};

var mainState = {
    buildingDestroyed: false,
    allowFire: false,
    shotsFired: 0,
    enemyFired: 0,
    enemyDead: 0,
    maxShots: 0,
    maxEnemy: 0,
    hits: 0,
    maxHits: 0,
    enemySpeed: MSpeed.Normal,
    successMessage: "",
    failureMessage: "",
    levelIntroMessage: "",
    stageIndex: 3,
    preload: function () {

    },
    create: function () {

        this.allowFire = false;
        this.shotsFired = 0;
        this.enemyFired = 0;
        this.enemyDead = 0;
        this.hits = 0;
        this.buildingDestroyed = false;

        var s = new SceneBuilder(this);

        this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";
        this.enemyremaininglbl.text = this.maxEnemy + " Missiles";
        this.help.setText(this.levelIntroMessage);
        this.help.alpha = 1;
        this.help.y = game.world.centerY - 200;

        emitter = game.add.emitter(game.world.centerX, 0, 400);

        //setup emitter for rain and other particles
        emitter.width = game.world.width;
        emitter.height = game.world.height - scenery.launcher.height;
        // emitter.angle = 30; // uncomment to set an angle for the rain.

        emitter.makeParticles('rain');

        emitter.minParticleScale = 0.1;
        emitter.maxParticleScale = 0.5;

        emitter.setYSpeed(300, 500);
        emitter.setXSpeed(-5, 5);

        emitter.minRotation = 0;
        emitter.maxRotation = 0;

        this.startstage.events.onInputDown.add(function (item) {
            var tween = game.add.tween(item).to({ alpha: 0 }, 600, Phaser.Easing.Linear.None, true, 300);
            tween.onComplete.add(function (sprite, tween) {
                startSound();
                sprite.destroy();
                this.help.destroy();
                mainState.allowFire = true;
                game.time.events.loop(3000, this.createBomb, this);

            }, this);
        }, this);

        game.time.events.loop(20000, this.setDayCycle, this);
        game.time.events.loop(30000, this.startRaining, this);
    },
    update: function () {
        this.cleanUp();
        if (game.input.activePointer.isDown && this.allowFire) {
            this.fire(game.input.activePointer.x, game.input.activePointer.y);
        }

        game.physics.arcade.collide(this.bombs, this.ground, this.bombCollisionWithGround, null, this);
        game.physics.arcade.collide(this.bombs, this.friends, this.bombCollisionWithFriend, null, this);
        game.physics.arcade.overlap(this.bombs, this.explosions, this.bombCollisionWithExplosion, null, this);
        game.physics.arcade.overlap(this.bd1layer, this.bombs, this.enemyhitbd1layer, null, this);
        game.physics.arcade.overlap(this.bd2layer, this.bombs, this.enemyhitbd2layer, null, this);
        game.physics.arcade.overlap(this.bd6layer, this.bombs, this.enemyhitbd6layer, null, this);
        game.physics.arcade.overlap(this.bd7layer, this.bombs, this.enemyhitbd7layer, null, this);
        game.physics.arcade.overlap(this.bd8layer, this.bombs, this.enemyhitbd8layer, null, this);
        game.physics.arcade.overlap(this.bd5layer, this.bombs, this.enemyhitbd5layer, null, this);
        game.physics.arcade.overlap(this.bd14layer, this.bombs, this.enemyhitbd14layer, null, this);
        game.physics.arcade.overlap(this.bd10layer, this.bombs, this.enemyhitbd10layer, null, this);
        game.physics.arcade.overlap(this.bd3layer, this.bombs, this.enemyhitbd3layer, null, this);
        game.physics.arcade.overlap(this.bd12layer, this.bombs, this.enemyhitbd12layer, null, this);
        game.physics.arcade.overlap(this.lotusside3, this.bombs, this.enemyhitlotusside3, null, this);
        game.physics.arcade.overlap(this.lotusside2, this.bombs, this.enemyhitlotusside2, null, this);
        game.physics.arcade.overlap(this.lotusside1, this.bombs, this.enemyhitlotusside1, null, this);
        game.physics.arcade.overlap(this.mstation, this.friends);

        if (this.hits >= this.maxHits) {
            this.allowFire = false;
            game.time.events.add(2000, function () { this.showFailurePopup() }, this);
        }
        if (this.hits < this.maxHits && this.enemyDead >= this.maxEnemy) {
            this.allowFire = false;
            game.time.events.add(2000, function () { this.showSuccessPopup(); }, this);
        }
    },
    setupExplosion: function (exp) {
        exp.anchor.x = 0.5;
        exp.anchor.y = 0.5;
        exp.x = -10;
        exp.y = -10;

        exp.animations.add('explode');
    },

    enemyhitbd1layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd1.frame < 3) {
            this.bd1.frame += 1;

        }
        if (this.bd1.frame == 2) {
            layer.height = layer.height / 2;
            layer.y += layer.height;

            this.hits += 1;
            this.showHealth();

        }

        if (this.bd1.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd2layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd2.frame < 3) {
            this.bd2.frame += 1;

        }
        if (this.bd2.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6363;
            layer.height = layer.height * 0.6363;
            this.hits += 1;
            this.showHealth();
        }

        if (this.bd2.frame == 2) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;
            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd6layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd6.frame < 3) {
            this.bd6.frame += 1;
        }
        if (this.bd6.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd6.frame == 2) {
            //multiply by ratio of thrid frame to second frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        if (this.bd6.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd7layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd7.frame < 2) {
            this.bd7.frame += 1;
        }
        if (this.bd7.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }

        if (this.bd7.frame == 2) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd8layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd8.frame < 3) {
            this.bd8.frame += 1;
        }
        if (this.bd8.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd8.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6;
            layer.height = layer.height * 0.6;
            this.hits += 1;
            this.showHealth();
        }

        if (this.bd8.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd5layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd5.frame < 3) {
            this.bd5.frame += 1;
        }
        if (this.bd5.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd5.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6;
            layer.height = layer.height * 0.6;

            this.hits += 1;
            this.showHealth();
        }

        if (this.bd5.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd14layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd14.frame < 3) {
            this.bd14.frame += 1;
        }
        if (this.bd14.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd14.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.6;
            layer.height = layer.height * 0.6;

            this.hits += 1;
            this.showHealth();
        }

        if (this.bd14.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd10layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd10.frame < 2) {
            this.bd10.frame += 1;
        }
        if (this.bd10.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }

        if (this.bd10.frame == 2) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd3layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd3.frame < 3) {
            this.bd3.frame += 1;
        }
        if (this.bd3.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd3.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;

            this.hits += 1;
            this.showHealth();
        }

        if (this.bd3.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;

            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitbd12layer: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        this.killEnemy(enemy);
        this.showPanickers(layer.x + (layer.width / 2));
        if (this.bd12.frame < 3) {
            this.bd12.frame += 1;
        }
        if (this.bd12.frame == 1) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;
            this.hits += 1;
            this.showHealth();
        }
        else if (this.bd12.frame == 2) {
            //multiply by ratio of second frame to first frame
            layer.y += layer.height - layer.height * 0.7;
            layer.height = layer.height * 0.7;
            this.hits += 1;
            this.showHealth();
        }

        if (this.bd12.frame == 3) {
            layer.enableBody = false;
            layer.kill();
            this.buildingDestroyed = true;
            this.hits += 1;
            this.showHealth();
        }
    },
    enemyhitlotusside1: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        enemy.blast();
        this.enemyFired += 1;
        layer.kill();

        this.lotusside2.destroy();
        this.lotusside3.destroy();
        this.lotus.frame = 3;
        this.buildingDestroyed = true;
        this.hits += 1;
        this.showHealth();
    },
    enemyhitlotusside2: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        enemy.blast();
        this.enemyFired += 1;
        layer.kill();
        this.lotusside3.destroy();
        this.showPanickers(layer.x + (layer.width / 2));

        this.lotus.frame = 2;
        this.hits += 1;
        this.showHealth();
    },
    enemyhitlotusside3: function (layer, enemy) {
        this.showExplosion(enemy.x, enemy.y);
        startBdExp();
        enemy.blast();
        this.enemyFired += 1;
        this.showPanickers(layer.x + (layer.width / 2));
        layer.kill();
        //layer.destroy();
        this.lotus.frame = 1;
        this.hits += 1;
        this.showHealth();
    },
    bombCollisionWithExplosion: function (bomb, explosion) {
        this.killEnemy(bomb);
    },
    bombCollisionWithGround: function (ground, bomb) {
        this.showExplosion(bomb.x, bomb.y);
        this.killEnemy(bomb);
        startBdExp();
    },
    showExplosion: function (x, y) {
        //var xpl = this.explosions.getFirstExists(false);
        //if (xpl) {
        //    xpl.reset(x, y);
        //    xpl.play('explode', 30, false, true);
        //}
        var xpl = this.explosions.create(x, y, 'explode');
        xpl.anchor.setTo(0.5, 0.5);

        xplanim = xpl.animations.add('xplode');
        xpl.animations.play('xplode', 65, false);
        xplanim.onComplete.add(function (sprite, animation) { sprite.destroy(); }, this);
    },
    setDayCycle: function () {
        if (this.bg1.alpha == 0) {
            game.add.tween(this.bg1).to({ alpha: 1 }, 2000).start();
            game.add.tween(this.bg2).to({ alpha: 0 }, 1000).start();
        }
        else {
            game.add.tween(this.bg1).to({ alpha: 0 }, 1000).start();
            game.add.tween(this.bg2).to({ alpha: 1 }, 2000).start();
        }
    },
    startRaining: function () {
        if (!raining) {
            raining = true;
            emitter.start(false, 1600, 5, 0);
            emitter.on = true;
        } else {
            raining = false;
            emitter.on = false;
        }
    },
    bombCollisionWithFriend: function (bomb, friend) {
        this.showExplosion(bomb.x, bomb.y);
        this.killEnemy(bomb);
        friend.blast();
    },
    fire: function (x, y) {
        if (this.shotsFired < this.maxShots) {
            if (game.time.now > nextFire) {
                var startpoint = { x: this.mstation.x + scenery.launcher.alfl, y: game.world.height - (scenery.ground.height + scenery.launcher.ahfg) };
                nextFire = game.time.now + fireRate;
                var f = new Friend(this.game, startpoint.x, startpoint.y, x, y);
                f.explosionSignal.add(function () {
                    this.showExplosion(f.x, f.y);

                }, this);
                this.shotsFired += 1;
                this.friendlyremaininglbl.text = (this.maxShots - this.shotsFired) + " Agni";

                if ((this.maxShots - this.shotsFired) <= 0) {
                    this.friendlyremaininglbl.stroke = scenery.noAgniWarningColor;
                    this.friendlyremaininglbl.strokeThickness = 6;
                }
                this.friends.add(f);
            }
        }
    },
    selfCall: function (f, delay) {
        var p = f.path.shift();
        if (p !== undefined) {
            f.rotation = game.physics.arcade.angleToXY(f, p.x, p.y);
            var tween = game.add.tween(f).to({ x: p.x, y: p.y }, delay, Phaser.Easing.Linear.None, true, 300);
            tween.onComplete.add(function () {
                f.accelerate = true;
            }, this);
        }
    },
    createBomb: function () {
        if (this.enemyFired < this.maxEnemy) {
            var x = Math.floor((Math.random() * game.world.width) + 1);
            var x2 = Math.floor((Math.random() * (game.world.width)) + 1);
            var missile = new EnemyMissile(this.game, x, -200, x2, game.world.height, MSpeed.GetAny());
            missile.fire();
            this.enemyFired += 1;
            this.bombs.add(missile);
        }

        this.enemyremaininglbl.text = ((this.maxEnemy - this.enemyFired) >= 0 ? (this.maxEnemy - this.enemyFired) : "0") + " Missiles";
        if ((this.maxEnemy - this.enemyFired) <= 0) {
            this.enemyremaininglbl.stroke = scenery.noAgniWarningColor;
            this.enemyremaininglbl.strokeThickness = 6;
        }
    },
    cleanUp: function () {
        this.bombs.forEachDead(function (item) {
            item.destroy();
        });
        this.friends.forEachDead(function (item) {
            item.destroy();
        });
        this.explosions.forEachDead(function (item) {
            item.destroy();
        });
        this.panickers.forEachDead(function (item) {
            item.destroy();
        });
    },
    showPanickers: function (x) {
        var no = Math.floor((Math.random() * 5) + 1);
        for (iCount = 0; iCount <= no; iCount++) {

            game.time.events.add(Math.floor((Math.random() * 1000) + 1), function () {
                var p = new Panicker(game, x, game.world.height - scenery.ground.height);
                this.panickers.add(p);
            }, this);

        }
    },
    killEnemy: function (sprite) {
        sprite.blast();
        this.enemyDead += 1;
    },
    showSuccessPopup: function () {
        var p = new Popup(this, this.successMessage, "Next", function (item) {
            stopStartSound();
            if (parseInt(localStorage.stageCleared, 10) <= this.stageIndex) {
                localStorage.stageCleared = this.stageIndex + 1;
            }

            game.state.start('stage');
        });
    },
    showFailurePopup: function () {
        var p = new Popup(this, this.failureMessage, "Next", function (item) {
            stopStartSound();
            game.state.start('stage');
        });

    },
    showHealth: function () {
        if (this.hits > 1) {
            this.healthwarninglbl.text = this.hits + " hits";
        } else {
            this.healthwarninglbl.text = this.hits + " hit";
        }
    },
};

var Friend = function (game, x, y, x2, y2) {
    Phaser.Sprite.call(this, game, x, y, 'friend');
    this.width = scenery.friend.width;
    this.height = scenery.friend.height;
    this.explosionSignal = new Phaser.Signal();
    this.anchor.setTo(0, 0.5);
    this.game.physics.enable(this, Phaser.Physics.ARCADE);
    this.body.allowGravity = false;
    this.body.collideWorldBounds = false;
    this.checkWorldBounds = true;
    this.outOfBoundsKill = true;

    this.accelerate = false;
    this.target = { x: x2, y: y2 };

    this.SMOKE_LIFETIME = 2000;
    if (!this.smokeEmitter) {
        this.smokeEmitter = this.game.add.emitter(0, 0, 300);
    }
    this.smokeEmitter.gravity = 0;
    this.smokeEmitter.setXSpeed(0, 0);
    this.smokeEmitter.setYSpeed(-10, 0); // make smoke drift upwards

    // Make particles fade out after 1000ms
    this.smokeEmitter.setAlpha(1, 0, this.SMOKE_LIFETIME,
        Phaser.Easing.Linear.InOut);

    // Create the actual particles
    this.smokeEmitter.makeParticles('smoke');

    // Start emitting smoke particles one at a time (explode=false) with a
    // lifespan of this.SMOKE_LIFETIME at 50ms intervals
    this.smokeEmitter.start(false, this.SMOKE_LIFETIME, 0);
    this.smokeEmitter.on = true;

    this.rotation = game.physics.arcade.angleToXY(this, x, y - 100);
    var tween = game.add.tween(this).to({ x: x, y: y - 100 }, 600, Phaser.Easing.Linear.None, true, 300);
    friendSound();
    tween.onComplete.add(function (sprite, tween) {
        sprite.accelerate = true;
    }, this);
};
// Missiles are a type of Phaser.Sprite
Friend.prototype = Object.create(Phaser.Sprite.prototype);
Friend.prototype.constructor = Friend;
Friend.prototype.update = function () {
    // Position the smoke emitter at the center of the missile
    this.smokeEmitter.x = this.x;
    this.smokeEmitter.y = this.y;

    if (this.accelerate) {
        var targetAngle = this.game.math.angleBetween(this.x, this.y, this.target.x, this.target.y);
        // Gradually (this.TURN_RATE) aim the missile towards the target angle
        if (this.rotation !== targetAngle) {
            // Calculate difference between the current angle and targetAngle
            var delta = targetAngle - this.rotation;

            // Keep it in range from -180 to 180 to make the most efficient turns.
            if (delta > Math.PI) delta -= Math.PI * 2;
            if (delta < -Math.PI) delta += Math.PI * 2;

            if (delta > 0) {
                // Turn clockwise
                this.angle += TURN_RATE;
            } else {
                // Turn counter-clockwise
                this.angle -= TURN_RATE;
            }

            // Just set angle to target angle if they are close
            if (Math.abs(delta) < this.game.math.degToRad(TURN_RATE)) {
                this.rotation = targetAngle;
            }
        }

        // Calculate velocity vector based on this.rotation and this.SPEED
        this.body.velocity.x = Math.cos(this.rotation) * SPEED;
        this.body.velocity.y = Math.sin(this.rotation) * SPEED;
        var distance = this.game.math.distance(this.x, this.y,
    this.target.x, this.target.y);
        if (distance < 20) {
            this.smokeEmitter.on = false;
            try {
                this.smokeEmitter.destroy();
            } catch (err) { }
            this.kill();
            this.explosionSignal.dispatch();
            stopFriendSound();
            startFriendExp();

        }
    }
};
Friend.prototype.blast = function () {
    stopFriendSound();
    startFriendExp();
    this.smokeEmitter.on = false;
    this.smokeEmitter.destroy();
    this.kill();
};

// Missile constructor
var EnemyMissile = function (game, x, y, targetx, targety, speed) {
    Phaser.Sprite.call(this, game, x, y, 'enemy');
    this.width = scenery.enemy.width;
    this.height = scenery.enemy.height;
    this.destinationSignal = new Phaser.Signal();
    this.destroySignal = new Phaser.Signal();
    this.speed = speed;
    // Set the pivot point for this sprite to the center
    this.anchor.setTo(0, 0.5);
    this.angle = 90;
    this.target = { x: targetx, y: targety };
    // Enable physics on the missile
    this.game.physics.enable(this, Phaser.Physics.ARCADE);

    // Define constants that affect motion
    this.SPEED = 250; // missile speed pixels/second
    this.TURN_RATE = 5; // turn rate in degrees/frame

    this.SMOKE_LIFETIME = 2000; // milliseconds


    // Add a smoke emitter with 100 particles positioned relative to the
    // bottom center of this missile
    if (!this.smokeEmitter) {
        this.smokeEmitter = this.game.add.emitter(0, 0, 100);
    }

    // Set motion paramters for the emitted particles
    this.smokeEmitter.gravity = 0;
    this.smokeEmitter.setXSpeed(0, 0);
    this.smokeEmitter.setYSpeed(-10, 0); // make smoke drift upwards

    // Make particles fade out after 1000ms
    this.smokeEmitter.setAlpha(1, 0, this.SMOKE_LIFETIME,
        Phaser.Easing.Linear.InOut);

    // Create the actual particles
    this.smokeEmitter.makeParticles('smoke');

    // Start emitting smoke particles one at a time (explode=false) with a
    // lifespan of this.SMOKE_LIFETIME at 50ms intervals
    this.smokeEmitter.start(false, this.SMOKE_LIFETIME, 50);
    this.smokeEmitter.on = true;
};
// Missiles are a type of Phaser.Sprite
EnemyMissile.prototype = Object.create(Phaser.Sprite.prototype);
EnemyMissile.prototype.constructor = EnemyMissile;
EnemyMissile.prototype.update = function () {
    // Position the smoke emitter at the center of the missile
    this.smokeEmitter.x = this.x;
    this.smokeEmitter.y = this.y;

    var distance = this.game.math.distance(this.x, this.y,
    this.target.x, this.target.y);
    if (distance < 5) {
        this.destinationSignal.dispatch();
    }
};
EnemyMissile.prototype.fire = function () {
    this.rotation = this.game.physics.arcade.moveToXY(this, this.target.x, this.target.y, this.speed);
}
EnemyMissile.prototype.blast = function () {
    this.smokeEmitter.on = false;
    this.smokeEmitter.destroy();
    this.destroySignal.dispatch();
    this.kill();
    //this.destroy();
}

var Panicker = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'panicker');

    this.anchor.setTo(0.5, 1);
    this.game.physics.enable(this, Phaser.Physics.ARCADE);
    panickeranim = this.animations.add('panickeranim');
    this.animations.play('panickeranim', 30, true);
    this.h = game.add.text(x - (this.width / 2), y - this.height - 10, PanickerShouts.GetAny(), scenery.panickerShoutStyle);
    this.h.anchor.setTo(0.5);
    game.add.tween(this.h).to({ alpha: 0 }, 2000, Phaser.Easing.Linear.None, true, 300);
    var direction = Math.floor((Math.random() * 10) + 1);
    if (direction % 2 == 0) {
        this.game.physics.arcade.moveToXY(this, game.world.width, this.position.y, MSpeed.GetAny());

    } else {
        this.scale.x *= -1;
        this.game.physics.arcade.moveToXY(this, 0, this.position.y, MSpeed.GetAny());
    }
}

Panicker.prototype = Object.create(Phaser.Sprite.prototype);
Panicker.prototype.constructor = Panicker;
Panicker.prototype.update = function () {
    this.h.x = this.x;
}

var SceneBuilder = function (state) {
    // Set the physics system
    state.game.physics.startSystem(Phaser.Physics.ARCADE);
    //capture moure for game
    state.game.input.mouse.capture = true;
    //Set night background 
    state.bg1 = state.game.add.sprite(0, 0, "bg1");
    state.bg1.width = game.width;
    state.bg1.height = game.height;

    state.bg2 = state.game.add.sprite(0, 0, "bg2");
    state.bg2.width = game.width;
    state.bg2.height = game.height;

    if (state.key == "tutorial" || state.key == "level2" || state.key == "level4" || state.key == "stage") {
        state.bg1.alpha = 1;
        state.bg2.alpha = 0;
    } else {
        state.bg1.alpha = 0;
        state.bg2.alpha = 1;
    }

    state.bgbuildings = state.game.add.sprite(0, game.world.height - (scenery.ground.height + scenery.bgbuildings.height), 'bgbuildings');
    state.bgbuildings.width = scenery.bgbuildings.width;
    state.bgbuildings.height = scenery.bgbuildings.height;

    //set ground for the game
    state.ground = state.game.add.sprite(0, game.world.height - scenery.ground.height, 'ground');
    state.ground.width = game.world.width;
    state.game.physics.arcade.enable(state.ground);
    state.ground.body.enable = true;
    state.ground.body.immovable = true;
    state.ground.body.allowGravity = false;

    if (state.key != "tutorial") {
        if (state.key != "stage") {
            state.lotus = state.game.add.sprite(scenery.lotus.x, game.world.height - (scenery.ground.height + scenery.lotus.height), 'lotus');
            state.lotus.width = scenery.lotus.width;
            state.lotus.height = scenery.lotus.height;

            state.lotusside1 = game.add.sprite(state.lotus.x + state.lotus.width / 2, game.world.height - scenery.ground.height - scenery.lotus.layer1.height, "spacer");
            state.lotusside1.anchor.setTo(0.5, 0);
            state.lotusside1.width = scenery.lotus.layer1.width;
            state.lotusside1.height = scenery.lotus.layer1.height;
            game.physics.arcade.enable(state.lotusside1);
            state.lotusside1.body.enable = true;
            state.lotusside1.body.immovable = true;
            state.lotusside1.body.allowGravity = false;

            state.lotusside2 = game.add.sprite(state.lotus.x + state.lotus.width / 2, state.lotusside1.y - scenery.lotus.layer2.height, "spacer");
            state.lotusside2.width = scenery.lotus.layer2.width;
            state.lotusside2.height = scenery.lotus.layer2.height;
            state.lotusside2.anchor.setTo(0.5, 0);
            game.physics.arcade.enable(state.lotusside2);
            state.lotusside2.body.enable = true;
            state.lotusside2.body.immovable = true;
            state.lotusside2.body.allowGravity = false;

            state.lotusside3 = game.add.sprite(state.lotus.x + state.lotus.width / 2, state.lotusside2.y - scenery.lotus.layer3.height, "spacer");
            state.lotusside3.anchor.setTo(0.5, 0);
            state.lotusside3.width = scenery.lotus.layer3.width;
            state.lotusside3.height = scenery.lotus.layer3.height;
            game.physics.arcade.enable(state.lotusside3);
            state.lotusside3.body.enable = true;
            state.lotusside3.body.immovable = true;
            state.lotusside3.body.allowGravity = false;

            //Show friend missile remaining count
            state.friendlyremaininglbl = game.add.text(10, 30, "0 Agni", scenery.subHeadingTextStyle);
            state.friendlyremaininglbl.anchor.setTo(0);

            //Show enemy missile remaining count
            state.enemyremaininglbl = game.add.text(game.world.width - 10, 30, "0 Missile", scenery.subHeadingTextStyle);
            state.enemyremaininglbl.anchor.setTo(1, 0);


            state.healthwarninglbl = game.add.text(game.world.centerX, 30, "", scenery.healthWarningStyle);
            state.healthwarninglbl.anchor.setTo(0.5, 0);

        }

        if (state.key == "level2" || state.key == "level3" || state.key == "level4" || state.key == "level5") {
            state.bd2 = state.game.add.sprite(scenery.bd2.x, game.world.height - (scenery.ground.height + scenery.bd2.height), 'bd2');
            state.bd2.width = scenery.bd2.width;
            state.bd2.height = scenery.bd2.height;
            state.bd2layer = game.add.sprite(state.bd2.x, state.bd2.y, "spacer");
            game.physics.arcade.enable(state.bd2layer);
            state.bd2layer.width = state.bd2.width;
            state.bd2layer.height = state.bd2.height;
            state.bd2layer.body.enable = true;
            state.bd2layer.body.immovable = true;
            state.bd2layer.body.allowGravity = false;


            state.bd8 = state.game.add.sprite(scenery.bd8.x, game.world.height - (scenery.ground.height + scenery.bd8.height), 'bd8');
            state.bd8.width = scenery.bd8.width;
            state.bd8.height = scenery.bd8.height;
            state.bd8layer = game.add.sprite(state.bd8.x, state.bd8.y, "spacer");
            game.physics.arcade.enable(state.bd8layer);
            state.bd8layer.width = state.bd8.width;
            state.bd8layer.height = state.bd8.height;
            state.bd8layer.body.enable = true;
            state.bd8layer.body.immovable = true;
            state.bd8layer.body.allowGravity = false;

            state.bd5 = state.game.add.sprite(scenery.bd5.x, game.world.height - (scenery.ground.height + scenery.bd5.height), 'bd5');
            state.bd5.width = scenery.bd5.width;
            state.bd5.height = scenery.bd5.height;
            state.bd5layer = game.add.sprite(state.bd5.x, state.bd5.y, "spacer");
            game.physics.arcade.enable(state.bd5layer);
            state.bd5layer.width = state.bd5.width;
            state.bd5layer.height = state.bd5.height;
            state.bd5layer.body.enable = true;
            state.bd5layer.body.immovable = true;
            state.bd5layer.body.allowGravity = false;

            if (state.key == "level4" || state.key == "level5" || state.key == "level3") {
                state.bd1 = state.game.add.sprite(scenery.bd1.x, game.world.height - (scenery.ground.height + scenery.bd1.height), 'bd1');
                state.bd1.width = scenery.bd1.width;
                state.bd1.height = scenery.bd1.height;
                state.bd1layer = game.add.sprite(state.bd1.x, game.world.height - (scenery.ground.height + scenery.bd1.height * scenery.bd1.lhratio), "spacer");
                game.physics.arcade.enable(state.bd1layer);
                state.bd1layer.width = scenery.bd1.width * scenery.bd1.lwratio;
                state.bd1layer.height = (scenery.bd1.height * scenery.bd1.lhratio);
                state.bd1layer.body.enable = true;
                state.bd1layer.body.immovable = true;
                state.bd1layer.body.allowGravity = false;

                state.bd6 = state.game.add.sprite(scenery.bd6.x, game.world.height - (scenery.ground.height + scenery.bd6.height), 'bd6');
                state.bd6.width = scenery.bd6.width;
                state.bd6.height = scenery.bd6.height;
                state.bd6layer = game.add.sprite(state.bd6.x, state.bd6.y, "spacer");
                game.physics.arcade.enable(state.bd6layer);
                state.bd6layer.width = state.bd6.width;
                state.bd6layer.height = state.bd6.height;
                state.bd6layer.body.enable = true;
                state.bd6layer.body.immovable = true;
                state.bd6layer.body.allowGravity = false;

                state.bd7 = state.game.add.sprite(scenery.bd7.X, game.world.height - (scenery.ground.height + scenery.bd7.height), 'bd7');
                state.bd7.width = scenery.bd7.width;
                state.bd7.height = scenery.bd7.height;
                state.bd7layer = game.add.sprite(state.bd7.x, state.bd7.y, "spacer");
                game.physics.arcade.enable(state.bd7layer);
                state.bd7layer.width = state.bd7.width;
                state.bd7layer.height = state.bd7.height;
                state.bd7layer.body.enable = true;
                state.bd7layer.body.immovable = true;
                state.bd7layer.body.allowGravity = false;

                state.bd14 = state.game.add.sprite(scenery.bd14.x, game.world.height - (scenery.ground.height + scenery.bd14.height), 'bd14');
                state.bd14.width = scenery.bd14.width;
                state.bd14.height = scenery.bd14.height;
                state.bd14layer = game.add.sprite(state.bd14.x, state.bd14.y, "spacer");
                game.physics.arcade.enable(state.bd14layer);
                state.bd14layer.width = state.bd14.width;
                state.bd14layer.height = state.bd14.height;
                state.bd14layer.body.enable = true;
                state.bd14layer.body.immovable = true;
                state.bd14layer.body.allowGravity = false;

                state.bd10 = state.game.add.sprite(scenery.bd10.x, game.world.height - (scenery.ground.height + scenery.bd10.height), 'bd10');
                state.bd10.width = scenery.bd10.width;
                state.bd10.height = scenery.bd10.height;
                state.bd10layer = game.add.sprite(state.bd10.x, state.bd10.y + state.bd10.height * 0.1, "spacer");
                game.physics.arcade.enable(state.bd10layer);
                state.bd10layer.width = state.bd10.width;
                state.bd10layer.height = state.bd10.height * 0.9;
                state.bd10layer.body.enable = true;
                state.bd10layer.body.immovable = true;
                state.bd10layer.body.allowGravity = false;

                state.bd3 = state.game.add.sprite(scenery.bd3.x, game.world.height - (scenery.ground.height + scenery.bd3.height), 'bd3');
                state.bd3.width = scenery.bd3.width;
                state.bd3.height = scenery.bd3.height;
                state.bd3layer = game.add.sprite(state.bd3.x, state.bd3.y + state.bd3.height * 0.1, "spacer");
                game.physics.arcade.enable(state.bd3layer);
                state.bd3layer.width = state.bd3.width;
                state.bd3layer.height = state.bd3.height * 0.9;
                state.bd3layer.body.enable = true;
                state.bd3layer.body.immovable = true;
                state.bd3layer.body.allowGravity = false;

                state.bd12 = state.game.add.sprite(scenery.bd12.x, game.world.height - (scenery.ground.height + scenery.bd12.height), 'bd12');
                state.bd12.width = scenery.bd12.width;
                state.bd12.height = scenery.bd12.height;
                state.bd12layer = game.add.sprite(state.bd12.x, state.bd12.y + state.bd12.height * 0.2, "spacer");
                game.physics.arcade.enable(state.bd12layer);
                state.bd12layer.width = state.bd12.width;
                state.bd12layer.height = state.bd12.height * 0.8;
                state.bd12layer.body.enable = true;
                state.bd12layer.body.immovable = true;
                state.bd12layer.body.allowGravity = false;
            }
        }

    }
    if (state.key != "stage") {
        //add missile station to game
        state.mstation = state.game.add.sprite((game.world.width / 2) - (scenery.launcher.width / 2), game.world.height - (scenery.launcher.height + scenery.ground.height), 'station');
        state.mstation.width = scenery.launcher.width;
        state.mstation.height = scenery.launcher.height;
        game.physics.arcade.enable(state.mstation);
        state.mstation.body.enable = true;
        state.mstation.body.immovable = true;
        state.mstation.body.allowGravity = true;

        state.friends = game.add.group();
        state.friends.enableBody = true;
        state.friends.physicsBodyType = Phaser.Physics.ARCADE;
        state.friends.setAll('outOfBoundsKill', true);

        state.panickers = game.add.group();
        state.panickers.enableBody = true;
        state.panickers.physicsBodyType = Phaser.Physics.ARCADE;
        state.panickers.setAll('outOfBoundsKill', true);

        state.explosions = game.add.group();
        state.explosions.enableBody = true;
        state.explosions.immovable = true;
        state.explosions.physicsBodyType = Phaser.Physics.ARCADE;

        state.bombs = game.add.group();
        state.bombs.enableBody = true;
        state.bombs.physicsBodyType = Phaser.Physics.ARCADE;

        state.startstage = game.add.text(game.world.centerX, game.world.centerY, "Start", scenery.levelNameStyle);
        state.startstage.anchor.set(0.5);
        state.startstage.inputEnabled = true;
        state.startstage.events.onInputOver.add(function () {
            state.startstage.fill = scenery.mouseOverTextColor;
            game.canvas.style.cursor = "pointer";
        }, state);

        state.startstage.events.onInputOut.add(function () {
            state.startstage.fill = scenery.mouseOutTextColor;
            game.canvas.style.cursor = "default";
        }, state);

        state.startstage.events.onInputDown.add(function () {

            game.canvas.style.cursor = "default";
        }, state);
        state.rubble = state.game.add.sprite(0, game.world.height - scenery.rubble.height, 'rubble');
        state.rubble.width = game.world.width;

        state.help = game.add.text(game.world.centerX, game.world.centerY, "Touch anywhere on the screen", scenery.commonTextStyle);
        state.help.anchor.set(0.5);
        state.help.alpha = 0;
    }
    else {
        //add volume control to stage choosing state only
        state.fullvolume = state.game.add.sprite(game.world.width - 50, 20, "fullvolume")
        state.fullvolume.anchor.setTo(0.5);
        state.fullvolume.inputEnabled = true;
        state.fullvolume.events.onInputDown.add(function () {
            localStorage.volumestatus = "false";

            state.fullvolume.visible = false;
            state.novolume.visible = true;
        }, state);

        state.novolume = state.game.add.sprite(game.world.width - 50, 20, "novolume")
        state.novolume.anchor.setTo(0.5);
        state.novolume.inputEnabled = true;
        state.novolume.events.onInputDown.add(function () {
            localStorage.volumestatus = "true";

            state.novolume.visible = false;
            state.fullvolume.visible = true;
        }, state);

        if (localStorage.volumestatus == "true") {
            state.novolume.visible = false;
            state.fullvolume.visible = true;
        } else {
            state.fullvolume.visible = false;
            state.novolume.visible = true;
        }
    }
    if (state.key == "tutorial") {
        state.pointer = state.game.add.sprite((game.world.width / 2) + 250, 300, 'pointer');
        state.pointer.anchor.set(0.5);
        state.pointer.alpha = 0;
        state.pointer.inputEnabled = true;
    }
    if (state.key != "stage") {
        bgsound = new Phaser.Sound(game, "bg", 0, true);
        agnisound = new Phaser.Sound(game, "agni", 0, false);
        expsound = new Phaser.Sound(game, "exp", 0, false);
        expbdsound = new Phaser.Sound(game, "expbd", 0.3, false);
        exphitsound = new Phaser.Sound(game, "exphit", 0.3, false);

        sounds = [bgsound, agnisound, expsound, expbdsound, exphitsound];
    }


}

function startSound() {
    if (localStorage.volumestatus == "true") {
        bgsound.play();
        bgsound.fadeTo(4000, 0.3);
    }
}

function stopStartSound() {
    bgsound.stop();
}

function friendSound() {
    if (localStorage.volumestatus == "true") {
        if (agnisound.isPlaying) {
            agnisound.stop();
        }
        agnisound.play();
        agnisound.fadeTo(3000, 0.3);
    }
}

function startFriendExp() {
    if (localStorage.volumestatus == "true") {
        exphitsound.play();
    }
}

function startBdExp() {
    if (localStorage.volumestatus == "true") {
        expbdsound.play();
    }
}

function startClickSound() {
    if (localStorage.volumestatus == "true") {
        clicksound.play();
    }
}

function stopFriendSound() {
    agnisound.stop();
}

var Popup = function (state, message, btnText, callback) {
    if (state.popup != undefined) {
        state.popup.destroy();
    }

    if (state.message != undefined) {
        state.message.destroy();
    }

    if (state.nextbtn != undefined) {
        state.nextbtn.destroy();
    }

    state.popup = state.game.add.sprite(game.world.centerX, game.world.centerY, 'popup');
    state.popup.anchor.set(0.5);

    state.message = state.game.add.text(state.popup.x, state.popup.y - 100, message, scenery.subHeadingTextStyle);
    state.message.anchor.set(0.5);

    state.nextbtn = state.game.add.text(state.popup.x, state.popup.y, btnText, scenery.buttonTextStyle);
    state.nextbtn.y = state.nextbtn.y + (state.popup.height / 2) - state.nextbtn.height;
    state.nextbtn.x = state.nextbtn.x + (state.popup.width / 2) - state.nextbtn.width;
    state.nextbtn.inputEnabled = true;
    state.nextbtn.anchor.setTo(0.5);
    state.nextbtn.events.onInputOver.add(function () {
        state.nextbtn.fill = scenery.mouseOverTextColor;
        game.canvas.style.cursor = "pointer";
    }, state);

    state.nextbtn.events.onInputOut.add(function () {
        state.nextbtn.fill = scenery.mouseOutTextColor;
        game.canvas.style.cursor = "default";
    }, state);
    state.nextbtn.events.onInputDown.add(callback, state);

}

game.state.add('init', initState);
game.state.add('stage', stageOptionState);
game.state.add('tutorial', tutorialState);
game.state.add('level1', level1State);
game.state.add('level2', level2State);
game.state.add('level3', mainState);
game.state.add('level4', mainState);
game.state.add('level5', mainState);
