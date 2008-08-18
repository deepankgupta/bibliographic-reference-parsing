package Biblio::Document::Parser::Standard;

require Exporter;
@ISA = ("Exporter", "Biblio::Document::Parser");

use Biblio::Document::Parser::Utils qw( normalise_multichars );

use 5.006;
use strict;
use warnings;
use vars qw($DEBUG);

our @EXPORT_OK = ( 'parse', 'new' );

$DEBUG = 0;

=pod

=head1 NAME

B<Biblio::Document::Parser::Standard> - document parsing functionality

=head1 SYNOPSIS

  use Biblio::Document::Parser::Standard;
  use Biblio::Document::Parser::Utils;
  # First read a file into an array of lines.
  my $content = Biblio::Document::Parser::Utils::get_content("http://www.foo.com/myfile.pdf");
  my $doc_parser = new Biblio::Document::Parser::Standard();
  my @references = $doc_parser->parse($content);
  # Print a list of the extracted references.
  foreach(@references) { print "-> $_\n"; } 

=head1 DESCRIPTION

Biblio::Document::Parser::Standard provides a fairly simple implementation of
a system to extract references from documents. 

Various styles of reference are supported, including numeric and indented,
and documents with two columns are converted into single-column documents
prior to parsing. This is a very experimental module, and still contains
a few hard-coded constants that can probably be improved upon.

=head1 METHODS

=over 4

=item $parser = Biblio::Document::Parser::Standard-E<gt>new()

The new() method creates a new parser instance.

=cut

sub new
{
        my($class) = @_;
        my $self = {};
        return bless($self, $class);
}

=pod

=item @references = $parser-E<gt>parse($lines, [%options])

The parse() method takes a string as input (see the get_content()
function in Biblio::Document::Parser::Utils for a way to obtain this), and returns a list
of references in plain text suitable for passing to a CiteParser module. 

=cut

sub parse
{
	my($self, $lines, %options) = @_;
	$lines = _addpagebreaks($lines);
	my @lines = split("\n", $lines);
	
	my $DEBUG = 1;
	
	my($pivot, $avelen) = $self->_decolumnise(@lines); 
	
	my $in_refs = 0;
	my @ref_table = ();
	my $curr_ref = "";
	my @newlines = ();
	my $outcount = 0;
	my @chopped_lines = @lines;
	# First isolate the reference array. This ensures that we handle columns correctly.
	foreach(@lines)
	{
		#print "\n Line : ";
		$outcount++;
		chomp;
		if (/^\s*Bibliography\s*$/i || /^\s*references\s*$/i || /^\s*References and Notes\s*$/i || /^\s*bibliographia\s*$/i || /^\s*bibliografia\s*$/i || /^\s*literatur\s*$/i )
                {
                        print 'Found';
                        last;
                }
		elsif (/\f/)
		{
			# No sign of any references yet, so pop off up to here
			for(my $i=0; $i<$outcount; $i++) { shift @chopped_lines; }
			$outcount = 0;
		}
	}
	my @arr1 = ();
	my @arr2 = ();
	my @arrout = ();
	my $indnt = "";

	my $prevnew = 0;
	foreach(@chopped_lines)
	{
		chomp;
		if (/^\s*(<p>)?\s*Bibliography\s*(<\/p>)?\s*$/i || /^\s*(<p>)?\s*references\s*(<\/p>)?\s*$/i || /^\s*(<p>)?\s*References and Notes\s*(<\/p>)?\s*$/i || /^\s*(<p>)?\s*bibliographia\s*(<\/p>)?\s*$/i || /^\s*(<p>)?\s*bibliografia\s*(<\/p>)?\s*$/i || /^\s*(<p>)?\s*literatur\s*(<\/p>)?\s*$/i )
                {
                        $in_refs = 1;
                        print "Found 2";
			push @newlines, $' if defined($'); # Capture bad input
                        next;
                }
		if (/^\s*\bappendix\b/i || /_{6}.{0,10}$/ || /^\s*\btable\b/i || /wish to thank/i || /\bfigure\s+\d/)
		{
			$in_refs = 0;
		}

		if (/^\s*$/)
		{
			if ($prevnew) { next; }
			$prevnew = 1;
		}
		else
		{
			$prevnew = 0;
		}

		if (/^\s*\d+\s*$/) { next; } # Page number

		if ($in_refs)
		{
			my $spaces = /^(\s+)/ ? length($1) : 0;
			if( @newlines && /^(\s+)[a-z]/ && _within(length($1),length($newlines[$#newlines]),5) ) {
				s/^\s+//s;
				$newlines[$#newlines] .= $_;
			} else {
				push @newlines, $_;
			}
		}
	}
	# We failed to find the reference section, we'll do a last-ditch effect at finding numbered
	# refs
	unless($in_refs) {
		my $first = 0;
		my $lastnum = 0;
		my $numwith = 0;
		my $numwo = 0;
		for(my $i = 0; $i < @chopped_lines; $i++) {
			$_ = $chopped_lines[$i];
			if( /^\s*[\[\(](\d+)[\]\)]/ || /^\s*(\d+)(?:\.|\s{5,})/ ) {
				$first = $1 if $1 == 1;
				if( $lastnum && $1 == $lastnum+1 ) {
					$numwo = 0;
					$numwith++;
					$lastnum++;
				} else {
					$first = $i;
					$lastnum = $1;
				}
			} elsif( $numwo++ == 5 ) { # Reset
				$first = $lastnum = $numwith = $numwo = 0;
			} elsif( $numwith == 5 ) {
				last;
			}
		}
		@newlines = splice(@chopped_lines,$first) if $first && $numwith == 5;
	}

warn "BEGIN REF SECTION\n", join("\n",@newlines), "\nEND REF SECTION\n";

    my $prev = "";
    foreach(@newlines)
	{
	    $_ = normalise_multichars($_);    #What does this function do????
		next if (/^(\s)*$/);
		
		s/(.*)(<br>)(.*)/$1 $3/;
		s/^\s*(<p>)*(.+)(<\/p>)+(\s)*$/$2/;
		print "$2\n";		
		if (length($2) > 30)
		{
      		push @ref_table, $2;
		}
		else
		{
		    #print "\n\nHere : ", length(@ref_table), "\n\n";
		    if (@ref_table && length($2) != 0)
		    {
		        $ref_table[-1] .= $2;
		    }
		    else
		    {
		        print "\n\nTHIS";
		        $ref_table[0] = $2;
		    }
		}
		$prev = $2;
	}

	my @refs_out = ();
	# A little cleaning up before returning
	my $prev_author;
	for (@ref_table)
	{
		s/([[:alpha:]])\-\s+/$1/g; # End of a line hyphen
		s/^\s*[\[\(]([^\]]+)[\]\)](.+)$/($1) $2/s;
		# Same author as previous citation
		$prev_author && s/^((?:[\(\[]\w+[\)\]])|(?:\d{1,3}\.))[\s_]{8,}/$1 $prev_author /;
		if( /^(?:(?:[\(\[]\w+[\)\]])|(?:\d{1,3}\.))\s*([^,]+?)(?:,|and)/ ) {
			$prev_author = $1;
		} else {
			undef $prev_author;
		}
		s/\s\s+/ /g;
		s/^\s*(.+)\s*$/$1/;
#		next if length $_ > 200;
		push @refs_out, $_;
	}
	return @refs_out;

}

# Private method to determine if/where columns are present.

sub _decolumnise 
{
	my($self, @lines) = @_;
	my @bitsout;
	my @lens = (0); # Removes need to check $lens[0] is defined
	foreach(@lines)
	{
		# Replaces tabs with 8 spaces
		s/\t/        /g;
		# Ignore lines that are >75% whitespace (probably diagrams/equations)
		next if( length($_) == 0 || (($_ =~ tr/ //)/length($_)) > .75 );
		# Split into characters
		my @bits = unpack "c*", $_;
		# Count lines together that vary slightly in length (within 5 chars)
		$lens[int(scalar @bits/5)*5+2]++;
		my @newbits = map { $_ = ($_==32?1:0) } @bits;
		for(my $i=0; $i<$#newbits; $i++) { $bitsout[$i]+=$newbits[$i]; } 
	}
	# Calculate the average length based on the modal.
	# 2003-05-14 Fixed by tdb
	my $avelen = 0;
	for(my $i = 0; $i < @lens; $i++ ) {
		next unless defined $lens[$i];
		$avelen = $i if $lens[$i] > $lens[$avelen];
	}
	my $maxpoint = 0;
	my $max = 0;
	# Determine which point has the most spaces
	for(my $i=0; $i<$#bitsout; $i++) { if ($bitsout[$i] > $max) { $max = $bitsout[$i]; $maxpoint = $i; } }
	my $center = int($avelen/2);
	my $output = 0;
	# Only accept if the max point lies around the average center.
	if ($center-6 <= $maxpoint && $center+6>= $maxpoint) { $output = $maxpoint; } else  {$output = 0;}
#warn "Decol: avelen=$avelen, center=$center, maxpoint=$maxpoint (output=$output)\n";
	return ($output, $avelen); 
}

# Private function that replaces header/footers with form feeds

sub _addpagebreaks {
	my $doc = shift;
	return $doc if $doc =~ /\f/s;
	my %HEADERS;

	while( $doc =~ /(?:\n[\r[:blank:]]*){2}([^\n]{0,40}\w+[^\n]{0,40})(?:\n[\r[:blank:]]*){3}/osg ) {
		$HEADERS{_header_to_regexp($1)}++;
	}

	if( %HEADERS ) {
		my @regexps = sort { $HEADERS{$b} <=> $HEADERS{$a} } keys %HEADERS;
		my $regexp = $regexps[0];
		if( $HEADERS{$regexp} > 3 ) {
			my $c = $doc =~ s/(?:\n[\r[:blank:]]*){2}(?:$regexp)(?:\n[\r[:blank:]]*){3}/\f/sg;
#			warn "Applying regexp: $regexp ($HEADERS{$regexp} original matches) Removed $c header/footers using ($HEADERS{$regexp} original matches): $regexp\n";
		} else {
			warn "Not enough matching header/footers were found ($HEADERS{$regexp} only)";
		}
	} else {
		warn "Header/footers not found - flying blind if this is a multi-column document";
	}

	return $doc;
}

sub _header_to_regexp {
	my $header = shift;
	$header =~ s/([\\\|\(\)\[\]\.\*\+\?\{\}])/\\$1/g;
	$header =~ s/\s+/\\s+/g;
	$header =~ s/\d+/\\d+/g;
	return $header;
}

sub _within {
	my ($l,$r,$p) = @_;
#warn "Is $l with $p of $r?\n";
	return $r >= $l-$p && $r <= $l+$p;
}

1;

__END__

=back
