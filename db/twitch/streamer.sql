create table twitch.streamer
(
    username          varchar(100) not null
        constraint streamer_pk
            primary key,
    started_streaming timestamp    not null
);

alter table twitch.streamer
    owner to markr;

