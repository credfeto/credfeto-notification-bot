create table twitch.stream
(
    channel    varchar(100) not null,
    start_date timestamp    not null,
    constraint stream_pk
        primary key (channel, start_date)
);

alter table twitch.stream
    owner to markr;

grant delete, insert, select, update on twitch.stream to notificationbot;

