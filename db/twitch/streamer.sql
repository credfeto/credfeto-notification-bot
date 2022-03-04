create table twitch.streamer
(
    username          VARCHAR(100) not null
        constraint streamer_pk
            primary key,
    started_streaming TIMESTAMP    not null
);

