local text = require('monoelib.ui.text')
local engine = require('monoelib.engine')
local mainwin = require('monoelib.io.mainwin')
local event = require('monoelib.event')
local system = require('monoelib.system.monsys')

function main()
  local label = text.new('This is the MONOE.exe engine !\n'
    .. 'If you see this, it means that the engine works (yeepee) !\n'
    .. 'You can see the work on Github here: https://github.com/YumStudioHQ/MONOE.exe'
  )

  label:font('!', 32)

  mainwin.title('monoe editor (best one hihi)')

  engine.qualify({
    root = label
  })
end

sentences = {
  'Miav!', 'bamm',
}

sentences_en = {
  "Hello there!",
  "The engine is alive.",
  "Rendering dreams...",
  "Pixels are thinking.",
  "This window feels powerful.",
  "Everything works. Probably.",
  "Reality compiled successfully.",
  "Running at full chaos.",
  "A label appeared!",
  "Something just happened.",
  "Time is passing.",
  "The loop never sleeps.",
  "Hello from the process event.",
  "Another frame, another hope.",
  "This text exists.",
  "Nothing is broken. Yet.",
  "Machines can smile too.",
  "Still running!",
  "The engine hums softly.",
  "Welcome to the void."
}

sentences_tr = {
  "Merhaba!",
  "Düzenek çalışıyor.",
  "Görüntü oluşuyor.",
  "Noktalar hareket ediyor.",
  "Bu pencere yaşıyor.",
  "Her şey yolunda.",
  "Zaman akıyor.",
  "Bir yazı belirdi.",
  "Bir şey oldu.",
  "Döngü sürüyor.",
  "Ses yok ama yaşam var.",
  "Işıklar yanıyor.",
  "Düşünce ekrana düştü.",
  "Bu satır burada.",
  "Düzen bozulmadı.",
  "Taşlar yerinde.",
  "Bak, çalışıyor!",
  "Yeni bir an.",
  "Gözler ekrana bakıyor.",
  "Her dönüş yeni bir iz."
}

sentences_jp = {
  "こんにちは！",
  "エンジンは生きている。",
  "画面が目を覚ました。",
  "文字が現れた。",
  "時間が流れている。",
  "何かが起きた。",
  "処理中…",
  "世界が動いている。",
  "新しいフレームだ。",
  "静かに動作中。",
  "小さな奇跡。",
  "表示成功。",
  "まだ走っている。",
  "ここに文字がある。",
  "問題なし。たぶん。",
  "画面の中で息をする。",
  "一瞬が生まれた。",
  "ループは続く。",
  "これは現実？",
  "エンジン、がんばれ！"
}

for _, t in ipairs(sentences_en) do table.insert(sentences, t) end
for _, t in ipairs(sentences_tr) do table.insert(sentences, t) end
for _, t in ipairs(sentences_jp) do table.insert(sentences, t) end

local timer = 0
local INTERVAL = 0.1

event.subscribe('process', function (delta)
  timer = timer + delta
  if timer >= INTERVAL then
    timer = timer - INTERVAL

    local label = text.new(sentences[math.random(#sentences)])
    engine.qualify({ root = label })

    label:position(mainwin:center())

    local wx, wy = mainwin.size()
    label:move(math.random(-(wx / 2), wx / 2), math.random(-(wy / 2), wy / 2))
    label:font('!', math.random(10, 40))
    label:color(math.random(), math.random(), math.random())

    local name = 'label#' .. label.uid
    system.timer.spawn(name, math.random(15), true)
    event.subscribe(name, function ()
      label:free()
    end)
  end
end)
