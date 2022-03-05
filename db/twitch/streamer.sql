create table twitch.streamer
(
    "UserName"    varchar(100) not null
        constraint streamer_pk
            primary key,
    "DateCreated" timestamp    not null
);

alter table twitch.streamer
    owner to markr;

