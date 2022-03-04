create table twitch.stream_chatter
(
    channel            varchar(100) not null,
    start_date         timestamp    not null,
    chat_user          varchar(100) not null,
    first_message_date timestamp    not null,
    constraint stream_chatter_pk
        primary key (channel, start_date, chat_user)
);

alter table twitch.stream_chatter
    owner to markr;

