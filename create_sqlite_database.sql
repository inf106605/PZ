CREATE TABLE friend
  (
    player_id_player_data INTEGER NOT NULL ,
    friend_id_player_data INTEGER NOT NULL ,
	PRIMARY KEY ( player_id_player_data, friend_id_player_data ) ,
	UNIQUE ( player_id_player_data , friend_id_player_data ) ,
	FOREIGN KEY ( player_id_player_data ) REFERENCES player_data ( id_player_data ) ,
	FOREIGN KEY ( friend_id_player_data ) REFERENCES player_data ( id_player_data )
  ) ;
CREATE INDEX friend__IDX_player ON friend
  ( player_id_player_data ASC
  ) ;
CREATE INDEX friend__IDX_friend ON friend
  ( friend_id_player_data ASC
  ) ;
CREATE INDEX friend__IDX_player_friend ON friend
  (
    player_id_player_data ASC ,
    friend_id_player_data ASC
  ) ;

CREATE TABLE game
  (
    id_game    INTEGER NOT NULL UNIQUE PRIMARY KEY ,
    begin_date TIMESTAMP NOT NULL ,
    end_date   TIMESTAMP NOT NULL ,
    game_log CLOB
  ) ;
CREATE UNIQUE INDEX game__IDX ON game
  (
    id_game ASC
  )
  ;

CREATE TABLE player
  (
    id_player                  INTEGER NOT NULL UNIQUE PRIMARY KEY ,
    player_data_id_player_data INTEGER NOT NULL UNIQUE ,
	FOREIGN KEY ( player_data_id_player_data ) REFERENCES player_data ( id_player_data )
  ) ;
CREATE UNIQUE INDEX player__IDX ON player
  (
    player_data_id_player_data ASC
  )
  ;

CREATE TABLE player_data
  (
    id_player_data INTEGER NOT NULL UNIQUE PRIMARY KEY ,
    nick           VARCHAR2 (50) NOT NULL ,
    login          VARCHAR2 (25) NOT NULL UNIQUE ,
    passwd_hash BLOB NOT NULL ,
    "e-mail"      VARCHAR2 (100) NOT NULL ,
    register_date TIMESTAMP NOT NULL ,
    last_act_date TIMESTAMP NOT NULL
  ) ;
CREATE UNIQUE INDEX player_data__IDX_id ON player_data
  (
    id_player_data ASC
  )
  ;
CREATE UNIQUE INDEX player_data__IDX_login ON player_data
  (
    login ASC
  )
  ;
  CREATE INDEX player_data__IDX_last_act ON player_data
    ( last_act_date ASC
    ) ;

CREATE TABLE score
  (
    walkover         CHAR (1) NOT NULL ,
    place            INTEGER NOT NULL ,
    score            INTEGER NOT NULL ,
    game_id_game     INTEGER NOT NULL ,
    player_id_player INTEGER NOT NULL ,
	PRIMARY KEY ( game_id_game, player_id_player ) ,
	UNIQUE ( game_id_game , player_id_player ) ,
	FOREIGN KEY ( game_id_game ) REFERENCES game ( id_game ) ,
	FOREIGN KEY ( player_id_player ) REFERENCES player ( id_player )
  ) ;
CREATE INDEX score__IDX_game ON score
  ( game_id_game ASC
  ) ;
CREATE INDEX score__IDX_player ON score
  ( player_id_player ASC
  ) ;
CREATE INDEX score__IDX_game_player ON score
  (
    game_id_game ASC ,
    player_id_player ASC
  ) ;


CREATE VIEW view_game  AS
SELECT game.*,
  COUNT(*)                      AS player_count,
  SUM(score.score)              AS score_sum,
  winner_score.player_id_player AS id_winner
FROM game
INNER JOIN score
ON game.id_game = score.game_id_game
INNER JOIN score winner_score
ON game.id_game          = winner_score.game_id_game
WHERE winner_score.place = 1
GROUP BY winner_score.player_id_player 
;

CREATE VIEW view_player  AS
SELECT player.*,
  COUNT(*)                      AS walkover_count,
  COUNT(*)                      AS games_count,
  AVG(non_walkover_score.score) AS avg_score
FROM player
INNER JOIN score walkover_score
ON player.id_player = walkover_score.player_id_player
INNER JOIN score games_score
ON player.id_player = games_score.player_id_player
INNER JOIN score non_walkover_score
ON player.id_player             = non_walkover_score.player_id_player
WHERE walkover_score.walkover   = 'true'
AND non_walkover_score.walkover = 'false' 
;

CREATE VIEW view_logged_player  AS
SELECT player_data.*,
  view_player.games_count,
  view_player.avg_score,
  view_player.walkover_count,
  view_player.id_player
FROM view_player
INNER JOIN player_data
ON view_player.id_player_data = player_data.id_player_data 
;
