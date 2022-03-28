create table twitch.streamer
(
    username     varchar(100) not null
        constraint streamer_pk
            primary key,
    date_created timestamp    not null,
    id           varchar(100) not null
);

alter table twitch.streamer
    owner to markr;

grant delete, insert, select, update on twitch.streamer to notificationbot;

