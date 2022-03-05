create table twitch.streamer
(
    username     varchar(100) not null
        constraint streamer_pk
            primary key,
    date_created timestamp    not null
);

alter table twitch.streamer
    owner to markr;

