create table twitch.stream_follower
(
    channel        varchar(100) not null,
    follower       varchar(100) not null,
    follow_count   integer      not null,
    first_followed timestamp    not null,
    last_followed  timestamp    not null
);

alter table twitch.stream_follower
    owner to markr;

grant insert, select, update on twitch.stream_follower to notificationbot;

