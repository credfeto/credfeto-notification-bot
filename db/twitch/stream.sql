create table twitch.stream
(
    channel    VARCHAR(100) not null,
    start_date timestamp    not null,
    constraint stream_pk
        primary key (channel, start_date)
);

