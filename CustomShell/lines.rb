require 'json'

files = Dir["*.cs"]
lines = 0
commented_lines = 0
puts files
for i in 0 ... files.length()
    lines += File.foreach(files[i]).inject(0) {|c, line| c+1}

    #if line.start_with?("//")
    #  commented_lines += 1
    #end
end

comment_percent = lines / commented_lines

puts "Total Lines: " + lines.to_s
puts "Percentage that's comments: " + comment_percent.to_s

raw_json = File.read('../Shields/lines.json')
hash = JSON.parse(raw_json)
hash['message'] = lines.to_s
File.write('../Shields/lines.json', JSON.dump(hash))
