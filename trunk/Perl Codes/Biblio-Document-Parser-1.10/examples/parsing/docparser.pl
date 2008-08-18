#!/usr/bin/perl

use utf8;
use lib "../..";

use Biblio::Document::Parser::Standard;
use Biblio::Document::Parser::Utils;
use Biblio::Citation::Parser::Standard;
use Biblio::Citation::Parser::Citebase;
use Biblio::Citation::Parser::Utils;
use Biblio::Citation::Parser::Jiao;
use Term::ANSIColor;

binmode(STDERR, ":utf8");
binmode(STDOUT, ":utf8");

if (scalar @ARGV != 1)
{
	print STDERR "Usage: $0 <filename>\n";
	exit;
}

my $doc_parser = new Biblio::Document::Parser::Standard;
#my $cit_parser = new Biblio::Citation::Parser::Standard;
#my $cit_parser = new Biblio::Citation::Parser::Citebase;
my $cit_parser = new Biblio::Citation::Parser::Jiao;

parse_document($doc_parser,$cit_parser);

sub parse_document {
	my ($doc_parser,$cit_parser) = @_;
	my $content = get_content($ARGV[0]);
	my @references = $doc_parser->parse($content);
	foreach $reference (@references)
	{
		$metadata = $cit_parser->parse($reference);
		$location = create_openurl($metadata);
        # Get a little extra info out of the metadata.

        print "- Metadata follows:\n";
        print "First name: ".$metadata->{aufirst}."\n";
        print "Last name: ".$metadata->{aulast}."\n";
        print "Title: ".$metadata->{atitle}."\n";
        print "Publication: ".$metadata->{title}."\n";
        print "Year: ".$metadata->{year}."\n";
        print "Issue: ".$metadata->{issue}."\n";
        print "Page range: ".$metadata->{pages}."\n";
        print "Matched template: ".$metadata->{match}."\n";
        print "Marked result: ".$metadata->{marked}."\n";
		print 
			color("red"), "$reference\n", color("reset"),
			color("green"), "\t$location\n", color("reset"),
			"---\n";
	}
}
