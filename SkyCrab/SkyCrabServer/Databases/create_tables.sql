CREATE TABLE database_info
  (
    lock char(1) not null PRIMARY KEY ,
	ver VARCHAR2 (10) NOT NULL ,
    CHECK (lock='X')
  ) ;

CREATE TABLE friend
  (
    player_id INTEGER NOT NULL ,
    friend_id INTEGER NOT NULL ,
	PRIMARY KEY ( player_id, friend_id ) ,
	FOREIGN KEY ( player_id ) REFERENCES player_profile ( player_id ) ,
	FOREIGN KEY ( friend_id ) REFERENCES player_profile ( player_id )
  ) ;
CREATE INDEX friend__IDX ON friend
  ( player_id ASC
  ) ;

CREATE TABLE game
  (
    game_id    INTEGER NOT NULL PRIMARY KEY ,
    begin_time TIMESTAMP NOT NULL ,
    end_time   TIMESTAMP ,
    game_log CLOB NOT NULL
  ) ;
CREATE UNIQUE INDEX game__IDX ON game
  (
    game_id ASC
  )
  ;

CREATE TABLE player
  ( player_id INTEGER NOT NULL PRIMARY KEY
  ) ;
CREATE UNIQUE INDEX player__IDX ON player
  (
    player_id ASC
  )
  ;

CREATE TABLE player_profile
  (
    login VARCHAR2 (25) NOT NULL UNIQUE ,
    password_hash BLOB NOT NULL ,
    nick          VARCHAR2 (50) NOT NULL ,
    "e-mail"      VARCHAR2 (100) NOT NULL UNIQUE ,
    register_data TIMESTAMP NOT NULL ,
    last_act_data TIMESTAMP NOT NULL ,
    player_id     INTEGER NOT NULL PRIMARY KEY ,
	FOREIGN KEY ( player_id ) REFERENCES player ( player_id )
  ) ;
CREATE UNIQUE INDEX player_profile__IDX ON player_profile
  (
    player_id ASC
  )
  ;
CREATE UNIQUE INDEX player_profile__IDX_login ON player_profile
  (
    login ASC
  )
  ;
CREATE UNIQUE INDEX "player_profile__IDX_e-mail" ON player_profile
  (
    "e-mail" ASC
  )
  ;

CREATE TABLE score
  (
    walkover  CHAR (1) NOT NULL ,
    score     INTEGER NOT NULL ,
    player_id INTEGER NOT NULL ,
    game_id   INTEGER NOT NULL ,
	PRIMARY KEY ( player_id, game_id ) ,
	FOREIGN KEY ( game_id ) REFERENCES game ( game_id ) ,
	FOREIGN KEY ( player_id ) REFERENCES player ( player_id )
  ) ;
CREATE UNIQUE INDEX score__IDX ON score
  (
    player_id ASC , game_id ASC
  )
  ;
  CREATE INDEX score__IDX_player ON score
    ( player_id ASC
    ) ;
  CREATE INDEX score__IDX_game ON score
    ( game_id ASC
    ) ;
