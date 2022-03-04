create table twitch.stream_chatter
(
    channel            VARCHAR(100) not null,
    start_date         TIMESTAMP    not null,
    chat_user          VARCHAR(100) not null,
    first_message_date TIMESTAMP    not null,
    constraint stream_chatter_pk
        primary key (channel, start_date, chat_user)
);

