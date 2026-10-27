# Lendas da Terra: A Jornada de Bento

Documento de Design de Jogo (GDD) — versão consolidada.

> **Protótipo (out/2026):** a fatia Norte já é jogável até o ritual do Curupira (ainda placa). Layout em faixas (Barca → Vila → Rio → Mata → Ruínas → Arena). Restauração, epílogo e resgate da fauna ainda pendem. Controles e regras atuais estão na [seção 12](#12-estado-atual-do-protótipo).

## 1. Visão geral

- **Gênero:** 2D Action-Adventure / Puzzle RPG side-scroller com progressão estilo metroidvania leve.
- **Engine:** Unity 6 (URP 2D), C#.
- **Plataformas:** PC, celular (Android/iOS) e web; consoles como meta futura.
- **Estilo visual:** aquarela 2D + ilustração brasileira (cordel, expedição botânica, arte naïf), inspirado em Spiritfarer.
- **Tom:** empático, cultural, aventureiro, defensor do patrimônio histórico e ambiental.
- **Idiomas:** português (principal) e inglês.

### Pilares
1. **Purificação, não extermínio.** As lendas foram corrompidas pelo Barão. Inimigos espirituais têm **corrupção (100% → 0%)**; industriais têm vida comum.
2. **Restauração do belo.** Áreas cinzentas, fumaça e água poluída voltam à aquarela viva e à música alegre após a purificação.
3. **Afeto e cotidiano.** Parallax com a vida do povo; momentos de acolhimento; NPCs que agradecem de forma comovente.
4. **Ciclo do pesquisador.** Pesquisar → conversar → comprar/escambar → preparar → ritual híbrido no chefe.

### Fórmula do protagonista
**Bento = Link (carisma + magia do amuleto + tiro) + Stella / Spiritfarer (afeto + restauração) + Indiana Jones (chicote + chapéu + astúcia + puzzles de ação).**

---

## 2. Protagonista: Bento

Etnógrafo, arqueólogo e folclorista. Curioso, respeitoso, perspicaz. Ganha na preparação e no cenário, não na força bruta.

### Visual de explorador
- Chapéu de abas largas / expedição (ajusta na cabeça ao resolver puzzle difícil).
- Jaqueta resistente, tiracolo e bolsa de couro (relíquias, anotações, reagentes).
- Visual levemente desgastado pelas viagens.
- Amuleto no peito (herança do avô — nome a definir).

### Manias e animações (polish)
- Limpar suor da testa ou assoprar poeira após escapar de armadilha.
- Reações cômicas / alívio; expressões nos diálogos (surpresa, dúvida, determinação).

### Movimento base (implementado)
Correr, pulo variável, **pulo duplo** (`pulosExtrasMax`, recarrega no chão e ao soltar o chicote), coyote time e buffer de pulo.

### Ferramentas

| Ferramenta | Função |
|---|---|
| **Chicote / corda com gancho** | Balançar; cruzar abismos; puxar alavancas; no combate: desarmar, puxar escudos, desequilibrar Sombras/capangas |
| **Facão de campo** | Abrir vegetação; combate corpo a corpo |
| **Besta Espiritual** | Disparo em arco (trajetória 2D); gatilhos; flechas elementais / purificadoras |
| **Amuleto Ancestral** | Visão Espectral (base); absorve fragmentos das lendas |
| **Caderno de Campo** | Diário ilustrado (não inventário frio): pistas, bestiário, receitas, diário do avô; abre no puzzle/boss para ver fraquezas |

### Loop de pesquisa (analógico)
Mesa de Pesquisa + diário do avô + manuscritos + relatos de NPCs. **Sem** dispositivo/rede digital.

### Economia
- **Escambo** com moradores (itens e favores).
- **Moeda** em feiras/mercadores.
- **Fundo de pesquisa:** dinheiro inicial por fase, com **teto por fase** (evita farm infinito).

---

## 3. Antagonista

**Barão Valério da Rocha** — magnata do extrativismo ilegal. Usa magia negra para corromper guardiões e culpar as lendas. Cada capítulo tem uma **Fortaleza Extrativista**.

### Inimigos
- **Sombras:** espíritos básicos da corrupção (roxos), purificáveis.
- **Capangas e máquinas:** inimigos industriais, perto das fortalezas.

### Chefes (modelo híbrido)
Não zerar “vida” por dano bruto. O jogador:
1. Consulta o Caderno (fraqueza).
2. Equipa fragmento / item ritual preparado.
3. Resolve o puzzle da arena e canaliza a purificação.
4. Combate Sombras/mecanismos invocados enquanto a **corrupção** desce.

---

## 4. Bases móveis

- **Barca de Rio:** Amazônia, São Francisco, Pantanal.
- **Kombi de Expedição:** Sertão, Sudeste, Sul.

Estações: **Fogão a Lenha**, **Bancada de Alquimia**, **Mesa de Pesquisa**. Base = salvamento. Após o Cap. 1, o **Mapa Geral do Brasil** permite revisitar regiões (side quests, compras, NPCs).

---

## 5. Diálogos e NPCs

- Caixa com retrato; cada fala tem personagem, texto e **emoção** (troca o retrato). Sem escolhas de resposta.
- Guia regional + NPCs de pista e afeto.

### Cap. 1 — Amazônia
| NPC | Papel |
|---|---|
| **Dona Nair** | Guia; Tacacá (Treme + oferenda ao Boto) e **fitas coloridas** para a Iara |
| **Namorada do Boto (Lia)** | Retrato para o Boto e **espelho** para a Iara; após purificação: feliz, apaixonada |
| **Pescador (Seu Zeca)** | **Pente de osso**; indica onde caiu a chave da canoa no cais (Visão Espectral) e testemunha o canto da Iara |

---

## 6. Amuleto — fragmentos

| Lenda | Fragmento | Função |
|---|---|---|
| *(base)* | Visão Espectral | Pegadas, símbolos, auras, pontos fracos de puzzle |
| **Curupira** | Passo Invertido | Ver caminhos falsos; vegetação cresce como plataformas |
| **Boto** | **Essência das Águas** | Energia espiritual no amuleto; necessária na conclusão da fase (com a Gota de Quartzo da Iara) |
| **Iara** | Gota de Quartzo Purificador | Mecanismos de água; purificar fluídos; ecos sob a água |
| **Boitatá** | Gema da Chama Fria | Luz no escuro; manipular fogo sem queimar |
| **Saci** | Fragmento do Redemoinho | Vento: mover objetos, cataventos, pressão |
| Cuca / Negrinho / outros | A definir | Capítulos 4–5 |

---

## 7. Culinária e efeitos

| Região | Pratos | Ingredientes | Efeito |
|---|---|---|---|
| **Norte** | Tacacá | Jambu + Tucupi + Goma | **Treme** (beber) contra o canto da Iara; **oferenda** ao Boto (não beber o último se for oferecer) |
| **Norte** | Pato no Tucupi | 2× Tucupi + Jambu | Variante do **Treme** (só buff; não serve de oferenda ao Boto) |
| Nordeste | Baião de Dois com carne de sol | Feijão fradinho, queijo coalho, carne de sol | **Vigor Sertanejo:** stamina (correr, chicote, escalar) |
| Centro-Oeste | Galinhada com pequi | Pequi, arroz, galinha caipira | **Olho de Águia:** alcance e precisão da besta |
| Sudeste | Feijoada completa | Feijão preto, porco, couve | **Casca Grossa:** escudo extra; menos dano de impacto |
| Sul | Barreado / Chimarrão | Carne lenta, farinha, erva-mate | **Foco e Aquecimento:** frio; recarga do amuleto |

---

## 8. Capítulos (mapa cultural)

| Cap. | Região | Chefe | Secundários | Base | Música |
|---|---|---|---|---|---|
| 1 | Norte (Amazônia) | Curupira | Boto, Iara | Barca | Carimbó, flautas, percussão |
| 2 | Nordeste | Cabeça de Cuia | Saci, Boitatá | Kombi e Barca | Samba de roda, Olodum, forró |
| 3 | Centro-Oeste | A definir | A definir | Kombi e Barca | Moda de viola |
| 4 | Sudeste | Cuca | Mula sem Cabeça | Kombi | Choro, bossa, samba |
| 5 | Sul | Negrinho do Pastoreio | Mboi Tu'i | Kombi | Chamamé, milonga |

### Direção de arte e som (resumo)
- **Parallax vivo:** cotidiano do povo na camada de fundo.
- **Luz/clima** por bioma (sertão ocre, pantanal esmeralda, costa serra-mar, etc.).
- **Trilha acústica** regional (sanfona, viola, cavaquinho, maracás…).

### Puzzles estilo Indiana Jones
Pisos falsos, pedras, dardos, luz/espelhos em ruínas (Marajoara, jesuítas, quilombolas); usar natureza (favo, rocha); furtividade para observar o chefe. Na fatia vertical: poucos exemplos; escala nos capítulos.

---

## 9. Capítulo 1 — O Clamor dos Igarapés (roteiro)

1. **Chegada de Barca.** Vila acinzentada: fumaça e incêndio ao fundo, água poluída, pesca e banho proibidos (Barão + corrupção). Dona Nair recebe Bento: avô, amuleto, receita do Tacacá (oferenda ao Boto + buff Treme) e **entrega o primeiro Tacacá pronto** mais **fitas coloridas** para a Iara. Lia dá o espelho; Seu Zeca, o pente de osso.
2. **Exploração** com chicote, facão e Visão Espectral; Sombras; momentos de afeto.
3. **Pistas.** Namorada do Boto + pescador da Iara alimentam o Caderno; Mesa de Pesquisa + diário do avô revelam o **Novelo de Cipó** para o Curupira.
4. **Rio: Boto + Iara.** Seu Zeca entrega o **pente de osso** e conta que a **chave da canoa** caiu na lama do cais — Bento usa a **Visão Espectral (`V`)** para revelá-la. O Boto é entidade espiritual: a poluição não o contamina, mas a dor dos animais o deixa incontrolável e ele tenta afundar o caíco. Na água turva, Bento usa a **Visão Espectral (`V`)** para enxergar a aura do Boto e acertar **três flechas purificadoras**; a oferenda completa é **Tacacá + foto da Lia**. O Boto envia a **Essência das Águas** ao amuleto. Sem desembarcar, a **Iara** desce da margem e canta: sem Treme, puxa o Bento ao fundo; com Treme, sente o amuleto. A oferenda dela é **pente + espelho + fitas** (licença ao rio, não comida). Entrega a **Gota de Quartzo**.
5. **Feira e escambo.** Cipó, resina, jambu, tucupi, goma; fundo de pesquisa com teto; moeda na feira.
6. **Preparo na Barca.** Novelo Encantado, flechas, Tacacá/Pato no Tucupi.
7. **Resgate da fauna** nas armadilhas do Barão.
8. **Ritual do Curupira (híbrido).** A **Gota de Quartzo** abre a arena. Visão Espectral → novelo nos focos → purificar Sombras (enquanto elas existem, a corrupção não desce) → corrupção a 0% → selo com **Gota + Essência das Águas**. Forma tradicional (menino, cabelo de fogo, pés invertidos). Recompensa: **Passo Invertido** no amuleto (habilidade em H03).
9. **Restauração + epílogo emocional.** Cor e carimbó voltam. Dona Nair (“seu avô deve estar orgulhoso”); crianças no rio; pescadores felizes; namorada do Boto feliz e apaixonada (o rapaz voltou às noites).
10. **Mapa do Brasil.** Seguir adiante; opcional voltar para side quests, compras e NPCs.

---

## 10. Diretrizes de arte e cultura

- IA só como **referência/conceito**; assets finais com consistência e animação.
- Pesquisar e creditar grafismos indígenas e tradições; evitar estereótipos.
- Steam: declarar IA se houver no build final.

### Nomes ainda abertos
- Nome do **amuleto** e do **avô**.
- Se o Barão tem cena direta no Cap. 1 ou só a Fortaleza ao fundo.

---

## 11. Roteiro técnico da fatia vertical

1. ~~Movimento + chicote (balanço) + pulo duplo~~ ✅
2. ~~Câmera (Cinemachine + limites)~~ ✅
3. ~~Vida, dano, checkpoint, zona de morte~~ ✅
4. UI — vida, munição, buffs, energia do amuleto, barra de corrupção do Curupira ✅
5. ~~Diálogo base + Dona Nair + Lia + Seu Zeca~~ ✅
6. Inventário ✅ · falta escambo, moeda e fundo de pesquisa
7. ~~Combate (facão, besta, Sombra, capanga) + chicote de combate/puzzle~~ ✅
8. ~~Caderno de Campo + Mesa de Pesquisa (diário do avô)~~ ✅
9. ~~Visão Espectral~~ ✅
10. Barca (Fogão, Alquimia) ✅ · falta salvamento JSON
11. ~~Encontros Boto + Iara~~ ✅ (canoa, flechas, Tacacá + foto; canto + Treme + pente/espelho/fitas)
12. ~~Layout Cap. 1 em faixas + ruína (alavanca, pisos falsos, relíquia)~~ ✅
13. Resgate da fauna
14. ~~Chefe Curupira (ritual H01, ainda placa)~~ ✅
15. Restauração + epílogo emocional
16. Mapa Geral (mínimo: Norte liberado)
17. Arte, áudio, Input System / toque, playtest

---

## 12. Estado atual do protótipo

Ao apertar Play, o `BootstrapPrototipo` monta a fase cinza. O `LayoutPrototipo` espalha tudo da esquerda para a direita.

### Mapa (esquerda → direita)

| Faixa | O que tem |
|---|---|
| **Barca** | Entrar com E. Fogão, Alquimia, Mesa de Pesquisa |
| **Vila** | Dona Nair (Tacacá + fitas), jambu/tucupi/goma, Lia (foto + espelho), chicote inicial |
| **Rio** | Seu Zeca (chave + pente), canoa, Boto, Iara, ingredientes extras na margem |
| **Mata** | Cipó, vegetação cortável, resina, capanga, Sombra, plataforma espectral |
| **Ruínas** | Alavanca de chicote, pisos falsos (Visão Espectral), relíquia Marajoara |
| **Arena** | Portão (Gota de Quartzo), 3 focos espectrais, Sombras, Curupira **placa** com ritual híbrido |

### Controles

| Tecla | Ação |
|---|---|
| A / D | Andar (na canoa: remar) |
| Espaço | Pular / pulo duplo / soltar chicote com impulso / avançar diálogo |
| E | Interagir (NPC, coletar, estações, canoa, oferenda, focos, portão). Clique direito: chicote |
| W / S | Recolher / alongar a corda |
| J ou clique esquerdo | Facão |
| K (segurar) | Mira da besta em arco; soltar dispara |
| V (segurar) | Visão Espectral (gasta energia do amuleto) |
| I | Inventário (Comer aplica Treme) |
| Tab ou C | Caderno de Campo |

### Regras dos encontros (implementadas)

**Canoa.** Precisa da chave de Seu Zeca (revelada no cais com Visão Espectral `V`). Três impactos do Boto afundam o caíco (respawn no checkpoint do rio). Só desembarca em cais.

**Boto.** Entidade espiritual: a poluição não o contamina; a dor dos animais o deixa incontrolável. Na zona, investe contra a canoa oculto na água turva. **Visão Espectral (`V`)** revela sua aura espiritual para acertar **3 flechas purificadoras**. Oferenda: **Tacacá + foto da Lia** (E). Envia a **Essência das Águas** ao amuleto (não ocupa a bolsa).

**Iara.** Sem desembarcar, desce da margem e canta. Sem Treme, em ~6 s puxa o Bento ao fundo. Com Treme (beber Tacacá ou Pato no inventário, inclusive com E durante o canto), ela para. Oferenda de licença: **pente + espelho + fitas** (não comida). Entrega a **Gota de Quartzo**.

**Alquimia.** Flechas = 1 resina + 1 cipó → 3 flechas. Novelo = 2 cipó + 1 resina.

**Curupira (ainda placa).** A Gota abre o portão da arena (não consome). Visão Espectral revela 3 focos; E enrola o Novelo (consome no primeiro foco). Cada foco invoca uma Sombra. Enquanto houver Sombra na arena, a corrupção não desce. Facão/besta no chefe não funcionam. Com 3 focos + 0 Sombras + corrupção a 0%, E sela com **Gota (consome) + Essência das Águas**. O amuleto recebe o **Passo Invertido** (habilidade ainda não implementada).

**Amuleto.** Fragmentos (Essência das Águas, Passo Invertido) não vão para a bolsa; a HUD lista os que o Bento já absorveu.

### Entrega de NPCs (primeira conversa)

| NPC | Entrega |
|---|---|
| Dona Nair | 1 Tacacá + fitas coloridas |
| Lia | Foto da Lia + espelho |
| Seu Zeca | Pente de osso (chave da canoa fica oculta na lama do cais até usar `V`) |

### Ainda placeholder

- Arte do Curupira (ainda placa) e habilidade Passo Invertido
- Restauração do belo e epílogos emocionais
- Resgate da fauna
- Escambo, moeda, fundo de pesquisa, save/load
- Retratos desenhados (diálogos ainda são OnGUI)
