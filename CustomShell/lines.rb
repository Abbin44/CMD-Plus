require 'json'

files = Dir["*.cs"]
lines = 0
commented_lines = 0
comment_percent = 0
total_lines = 0
puts files
for i in 0 ... files.length()
  if not files[i].end_with?(".Designer.cs")
      File.open(files[i]).each(sep="\n") do |line|

      total_lines += 1

      line.strip!
      if line.start_with?("//")
        commented_lines += 1
        puts line
      else
        lines +=1
      end
    end
  end
end

comment_percent = (commented_lines.to_f / total_lines.to_f) * 100
puts "Total Lines: " + total_lines.to_s
puts "Percentage that's comments: " + comment_percent.to_s + "%"

raw_json = File.read('../Shields/lines.json')
hash = JSON.parse(raw_json)
hash['message'] = total_lines.to_s
File.write('../Shields/lines.json', JSON.dump(hash))
