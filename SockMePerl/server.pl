#!/usr/bin/env perl
use v5.30;
use warnings;

use IO::Socket::IP;

my $server = IO::Socket::IP->new(
    Domain => AF_INET,
    Type => SOCK_STREAM,
    Proto => 'tcp',
    LocalHost => '0.0.0.0',
    LocalPort => 6969,
    ReusePort => 1,
    Listen => 5,
) || die "Can't open socket: $IO::Socket::errstr";
say "Waiting on 6969 for something to happen.";

while (1) {
    my $client = $server->accept();

    my $client_address = $client->peerhost();
    my $client_port = $client->peerport();
    say "Connection from $client_address:$client_port";

    my $data = "";
    $client->recv($data, 1024);
    say "received data: $data";

    $data = "ok";
    $client->send($data);

    $client->shutdown(SHUT_WR);
}

$server->close();
