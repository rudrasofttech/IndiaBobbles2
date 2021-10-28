// Initialize Phaser, and creates a 400x490px game
var game = new Phaser.Game(document.getElementById("gameDiv").offsetWidth, document.getElementById("gameDiv").offsetHeight, Phaser.AUTO, 'gameDiv');
var score = 0;
var showhighscorenotification = true;
//variable hold value whether play should live and is allowed to do his action
var die = false;
//object to hold player destoryed animation
var leftclicked = false, rightclicked = false;
var playerface = "right";
var movedirection = "";

var meteorclosenesslimit = 50;
var meteorcloseness = 500;
var meteorclosenessincreaserate = 1;
var meteoraccincreaserate = 5;

var initState = {
    preload: function () {
        game.load.image("splashscreen", "images/Splash-Screen.jpg");
        //load background image for the game
        game.load.image("background", "images/BG_1366x768.jpg");
        //load play sprite
        game.load.spritesheet('player', 'images/Rover_Sprites.png', 95, 120);
        //game.load.image('header', 'assets/header.png');
        game.load.spritesheet('explode', 'images/Efx_Explosion_v02.png', 50, 76);
        game.load.spritesheet('dai', 'images/Sprites_BlueGem_42x50.png', 42, 50);
        game.load.spritesheet('reddai', 'images/redgem-sprite.png', 44, 50);
        game.load.spritesheet('bomb', 'images/Meteor_Sprites.png', 48, 95);
        game.load.image('ground', 'images/Ground_1366x768.png');
        game.load.image('dirt', 'images/dirtlayer.png');
        game.load.image('scorepanel', 'images/score-panel.png');
        game.load.image('scorepopup', 'images/popup.png');
        game.load.spritesheet('left', 'images/left.png', 98, 103);
        game.load.spritesheet('right', 'images/right.png', 98, 103);
        game.load.spritesheet('pause', 'images/pause.png', 76, 79);
        game.load.spritesheet('play', 'images/play.png', 76, 79);
        game.load.spritesheet('fullvolume', 'images/fullvolume.png', 77, 77);
        game.load.spritesheet('novolume', 'images/novolume.png', 77, 77);
        if (playsound) {
            game.load.audio('rovermoving', 'sound/Rover moving new 3.mp3');
            game.load.audio('gameplay', 'sound/Sci Fi theme 4.mp3');
            game.load.audio('gameend', 'sound/game ending.mp3');
            game.load.audio('collectinggem', 'sound/Gem Collecting 3.mp3');
            game.load.audio('explosionsound', 'sound/explosion 2.mp3');
            game.load.audio("startbutton", 'sound/Start button.mp3');
        }
    },
    create: function () {
        this.background = this.game.add.sprite(game.world.centerX, game.world.centerY, "splashscreen");
        this.background.width = splashscreenwidth;
        this.background.height = splashscreenheight;
        this.background.anchor.setTo(0.5, 0.5);
        this.startbutton = game.add.audio("startbutton");
        score = 0;
        var text2 = subtextobj.text;
        var style2 = subtextobj.fontstyle;
        var t2 = game.add.text(game.world.centerX, subtextobj.margintop, text2, style2);
        t2.anchor.setTo(0.5, 0.5);
        t2.alpha = 0;

        game.input.onDown.addOnce(function () {
            if (playsound && localStorage.volume == 1) {
                this.startbutton.play();
            }
            var t2tween = game.add.tween(t2).to({ alpha: 1 }, 3000, "Linear", true);

            t2tween.onComplete.add(function () {
                game.state.start('main');
            }, this);

        }, this);
    }
};

// Creates a new 'main' state that will contain the game
var mainState = {
    // Function called first to load all the assets
    preload: function () {

    },

    // Fuction called after 'preload' to setup the game 
    create: function () {
        die = false;
        score = 0;
        showhighscorenotification = true;
        meteoracceleration = 200;
        // Set the physics system
        game.physics.startSystem(Phaser.Physics.ARCADE);

        this.rovermoving = game.add.audio("rovermoving");
        this.gameplay = game.add.audio("gameplay");
        if (playsound && localStorage.volume == 1) {
            this.gameplay.play("", 0, 0.3, true, true);
        }

        this.gameend = game.add.audio("gameend");
        this.collectinggem = game.add.audio("collectinggem");
        this.explosionsound = game.add.audio("explosionsound");
        this.background = this.game.add.sprite(0, 0, "background");
        this.background.width = game.world.width;
        this.background.height = game.world.height + 25;

        this.play = game.add.button(game.world.width - playbtnwidth, 10, 'play', null, this, 0, 0, 0, 0);
        this.play.width = playbtnwidth;
        this.play.height = playbtnheight;
        this.play.visible = false;

        this.pause = game.add.button(game.world.width - playbtnwidth, 10, 'pause', null, this, 0, 0, 0, 0);
        this.pause.width = playbtnwidth;
        this.pause.height = playbtnheight;
        this.pause.events.onInputUp.add(function () { this.setPause(); }, this);

        this.fullvolume = game.add.button(game.world.width - volumebtnwidth - this.pause.width, 10, "fullvolume", null, this, 0, 0, 0, 0);
        this.fullvolume.width = volumebtnwidth;
        this.fullvolume.height = volumebtnheight;
        this.fullvolume.events.onInputUp.add(function () {
            //on click toggle volume to zero
            localStorage.volume = 0;
            this.fullvolume.visible = false;
            this.novolume.visible = true;

            this.gameplay.pause();
        }, this);

        this.novolume = game.add.button(game.world.width - volumebtnwidth - this.pause.width, 10, "novolume", null, this, 0, 0, 0, 0);
        this.novolume.width = volumebtnwidth;
        this.novolume.height = volumebtnheight;
        this.novolume.events.onInputUp.add(function () {
            //on click toggle volume to one
            localStorage.volume = 1;
            this.fullvolume.visible = true;
            this.novolume.visible = false;
            this.gameplay.resume();

        }, this);

        if (runningdevice == "desktop") {
            if (localStorage.volume == 0) {
                this.fullvolume.visible = false;
                this.novolume.visible = true;
            } else {
                this.fullvolume.visible = true;
                this.novolume.visible = false;
            }

        } else if (runningdevice == "mobile") {
            this.fullvolume.visible = false;
            this.novolume.visible = false;
        }

        // Add a input listener that can help us return from being paused
        game.input.onDown.add(function (event) {
            // Only act if paused
            if (game.paused) {

                game.paused = false;
                this.play.visible = false;
                this.pause.visible = true;
                if (playsound && localStorage.volume == 1) {
                    this.gameplay.play();
                }
            }
        }, this);
        game.input.onUp.add(function () { movedirection = ""; });

        this.ground = this.game.add.sprite(0, game.world.height - groundheight, 'ground');
        this.ground.width = game.world.width;
        this.ground.height = groundheight;
        game.physics.arcade.enable(this.ground);
        this.ground.body.enable = true;
        this.ground.body.immovable = true;
        this.ground.body.allowGravity = false;

        // Display the player on the screen
        this.player = this.game.add.sprite(game.world.centerX, game.world.height - (playerheight / 2 + this.ground.height), 'player', 1);
        this.player.width = playerwidth;
        this.player.height = playerheight;

        this.player.anchor.setTo(0.5, 0.5);

        /* [0,1,2,...,21] */
        framesright = [];
        for (i = 0; i < 24; i++) { framesright.push(i); }

        this.player.animations.add("walkright", framesright, 50, true, true);
        framesleft = [];
        for (i = 25; i < 48; i++) { framesleft.push(i); }

        this.player.animations.add("walkleft", framesleft, 50, true, true);
        framesdestoryright = [];
        for (i = 97; i < 108; i++) { framesdestoryright.push(i); }

        var pdhandle = this.player.animations.add("destoryright", framesdestoryright, 50, true, true);
        pdhandle.onComplete.add(function (sprite, animation) {
            this.showCasting();
        }, this);

        framesdestoryleft = [];
        for (i = 121; i < 132; i++) {
            framesdestoryleft.push(i);
        }
        var pdhandle2 = this.player.animations.add("destoryleft", framesdestoryleft, 50, true, true);
        pdhandle2.onComplete.add(function (sprite, animation) {
            this.showCasting();
        }, this);
        this.player.animations.add("standingright", [1], 50, true, true);
        this.player.animations.play('standingright', 1, true);

        this.dirt = this.game.add.sprite(0, game.world.height - (groundheight + 9), 'dirt');
        this.dirt.width = game.world.width;

        // Add gravity to the player to make it fall
        game.physics.arcade.enable(this.player);
        this.player.body.gravity.y = 100;
        this.player.body.collideWorldBounds = true;
        this.player.body.checkCollision.down = true;

        this.cursor = this.game.input.keyboard.createCursorKeys();

        this.bombs = game.add.group();
        this.bombs.enableBody = true;
        this.bombs.physicsBodyType = Phaser.Physics.ARCADE;

        this.gems = game.add.group();
        this.gems.enableBody = true;
        this.gems.physicsBodyType = Phaser.Physics.ARCADE;

        this.explosions = game.add.group();

        game.time.events.loop(350, this.createBomb, this);

        this.scorepanel = this.game.add.sprite(game.world.width - (scorepanelwidth + this.pause.width + this.fullvolume.width + 10), 10, "scorepanel");
        this.scorepanel.width = scorepanelwidth;
        this.scorepanel.height = scorepanelheight;
        this.labelScore = this.game.add.text(this.scorepanel.x + 15, this.scorepanel.y + labelscorestyle.margintop, "0", labelscorestyle.fontstyle);

        this.notifypopup = game.add.text(game.world.centerX, notifypopupstyle.margintop, "", notifypopupstyle.fontstyle);
        this.notifypopup.anchor.setTo(0.5, 0.5);
        this.notifypopup.alpha = 0;

        if (localStorage.highscore > 0) {
            this.labelScore.setText("Beat " + localStorage.highscore);
        }

        if (runningdevice == "mobile") {
            this.leftpaddle = this.game.add.button(5, game.world.height - 105, "left", null, this, 0, 0, 0, 0);
            this.leftpaddle.alpha = 0.5;
            this.leftpaddle.events.onInputDown.add(function () { leftclicked = true; rightclicked = false; }, this);
            this.leftpaddle.events.onInputUp.add(function () { leftclicked = false; }, this);
            this.rightpaddle = this.game.add.button(game.world.width - 100, game.world.height - 105, "right", null, this, 0, 0, 0, 0);
            this.rightpaddle.alpha = 0.5;
            this.rightpaddle.events.onInputDown.add(function () { rightclicked = true; leftclicked = false; }, this);
            this.rightpaddle.events.onInputUp.add(function () { rightclicked = false; }, this);
        }
    },
    setPause: function () {
        this.gameplay.stop();
        game.paused = true;
        this.pause.visible = false;
        this.play.visible = true;
    },
    showCasting: function () {
        if (playsound && localStorage.volume == 1) {
            this.gameend.play("", 0, 1, false, true);
        }

        var scorepopup = game.add.sprite(game.world.centerX, game.world.centerY, "scorepopup");
        scorepopup.width = scorepopupwidth;
        scorepopup.height = scorepopupheight;

        scorepopup.anchor.setTo(0.5, 0.5);

        var text = score + "";
        var style = { font: "100px Trebuchet MS, Impact, Arial", fill: "#fff", align: "center" };

        var t = game.add.text(game.world.centerX, game.world.centerY + 25, text, style);
        t.anchor.setTo(0.5, 0.5);

        this.scorepanel.kill();
        this.labelScore.text = "";
        this.pause.visible = false;
        this.fullvolume.visible = false;
        this.novolume.visible = false;

        game.input.onDown.addOnce(function () { this.gameend.stop(); game.state.start('main'); }, this);
    },
    movePlayer: function () {
        if (!die) {
            if (this.player != null) {
                if (movedirection == "left") {
                    this.rovermoving.volume = 0.5;
                    this.player.body.velocity.x = (-roverdefaultspeed - score) < -rovermaxspeed ? -rovermaxspeed : (-roverdefaultspeed - score);
                    this.player.animations.play('walkleft', 12, true);
                    playerface = "left";

                }
                else if (movedirection == "right") {
                    this.rovermoving.volume = 0.5;
                    this.player.body.velocity.x = (roverdefaultspeed + score) > rovermaxspeed ? rovermaxspeed : (roverdefaultspeed + score);
                    this.player.animations.play('walkright', 12, true);
                    playerface = "right";

                }
                else {
                    this.rovermoving.volume = 0;
                    this.player.body.velocity.x = 0;
                    this.player.animations.stop(null, true);
                }
            }
        } else {
            this.rovermoving.volume = 0;
            this.player.body.velocity.x = 0;
            this.player.body.velocity.y = 0;
        }
    },

    // This function is called 60 times per second
    update: function () {
        game.physics.arcade.collide(this.player, this.ground);
        game.physics.arcade.collide(this.gems, this.ground);
        game.physics.arcade.overlap(this.player, this.gems, this.playerCollisionWithGem, null, this);
        game.physics.arcade.overlap(this.bombs, this.ground, this.bombCollisionWithGround, null, this);
        game.physics.arcade.collide(this.player, this.bombs, this.bombCollisionWithPlayer, null, this);
        game.physics.arcade.overlap(this.player, this.explosions);

        pd = game.input.pointer1.position;

        if (this.cursor.left.isDown || /*leftclicked ||*/ (game.input.pointer1.isDown && (pd.x > 0 && pd.x < 200))) {
            movedirection = "left";
        }
        else if (this.cursor.right.isDown || /*rightclicked ||*/ (game.input.pointer1.isDown && (pd.x > game.world.width - 200 && pd.x < game.world.width))) {
            movedirection = "right";
        }
        else {
            movedirection = "";
        }
        this.movePlayer();
    },
    createBomb: function () {
        if (!die) {
            var x = Math.floor((Math.random() * game.world.width) + 1);

            var bomb = this.bombs.create(x, -100, 'bomb', 1);
            bomb.width = bombwidth;
            bomb.height = bombheight;

            bomb.anchor.setTo(0.5, 0.5);
            game.physics.arcade.enable(bomb);

            if (maxmeteoracceleration > meteoracceleration) {
                meteoracceleration += meteoraccincreaserate;
            }
            bomb.body.gravity.y = meteorspeed;
            bomb.body.acceleration.y = meteoracceleration;

            bomb.body.collideWorldBounds = true

            bomb.checkWorldBounds = true;
            bomb.outOfBoundsKill = true;
            bombanim = [];
            for (i = 1; i < 5; i++) {
                bombanim.push(i);
            }
            bomb.animations.add('burning', bombanim, 50, true, true);
            bomb.animations.play('burning', 12, true);
        }
    },
    playerCollisionWithGem: function (player, gem) {
        if (playsound && localStorage.volume == 1) {
            this.collectinggem.play("", 0, 0.5, false, true);
        }
        score += gem.score;
        if (localStorage.highscore > score) {
            this.labelScore.text = score + " / " + localStorage.highscore;
        } else {
            this.labelScore.text = score;
        }

        if (parseInt(score, 10) > parseInt(localStorage.highscore, 10)) {
            localStorage.highscore = score;
            if (showhighscorenotification) {
                this.notifypopup.setText("New Highscore!");
                this.notifypopup.alpha = 1;
                showhighscorenotification = false;
                game.add.tween(this.notifypopup).to({ alpha: 0 }, 3000, "Linear", true);
            }
        }

        gem.kill();
        gem.destroy();
    },

    bombCollisionWithGround: function (ground, bomb) {
        var xpl = this.explosions.create(bomb.x, ground.y, 'explode', 1);

        xpl.anchor.setTo(0.5, 1);
        bomb.kill();
        xplframes = [];
        for (i = 1; i < 18; i++) {
            xplframes.push(i);
        }
        xplanim = xpl.animations.add('xplode', xplframes, 50, true, true);


        xpl.animations.play('xplode', 24, false);
        xplanim.onComplete.add(function (sprite, animation) {
            sprite.kill();
            var random = Math.floor((Math.random() * 10) + 1);
            if (random % 5 == 0) {
                if (Math.floor((Math.random() * 5) + 1) % 5 != 0) {
                    var gem = this.gems.create(sprite.x, sprite.y, "dai", 1);
                    gem.score = 1;
                    gem.width = gemwidth;
                    gem.height = gemheight;
                    gem.anchor.setTo(0.5, 1);
                    sparkleframes = [];
                    for (i = 1; i < 25; i++) {
                        sparkleframes.push(i);
                    }
                    gem.animations.add("sparkle", sparkleframes, 50, true, true);
                    gem.animations.play("sparkle", 12, true);
                } else {
                    var gem = this.gems.create(sprite.x, sprite.y, "reddai", 1);
                    gem.score = 10;
                    gem.width = gemwidth;
                    gem.height = gemheight;
                    gem.anchor.setTo(0.5, 1);
                    var redsparkleframes = [];
                    for (i = 1; i < 18; i++) {
                        redsparkleframes.push(i);
                    }
                    gem.animations.add("redsparkle", redsparkleframes, 50, true, true);
                    gem.animations.play("redsparkle", 12, true);
                }
            }
        }, this);

    },
    bombCollisionWithPlayer: function (player, bomb) {

        bomb.kill();
        bomb = null;
        if (!die) {

            die = true;
            this.gameplay.stop();
            if (playsound && localStorage.volume == 1) {
                this.explosionsound.play("", 0, 0.5, false, false);
            }
            if (playerface == "left") {
                this.player.animations.play('destoryleft', 8, false);
            } else if (playerface == "right") {
                this.player.animations.play('destoryright', 8, false);
            }
        }

    },
    explosionCollisionWithPlayer: function (player, explosion) { },

    // Restart the game
    restartGame: function () {
        // Start the 'main' state, which restarts the game
        game.state.start('init', true, true);
    }
};

game.state.add('main', mainState);
game.state.add('init', initState);
game.state.start('init');