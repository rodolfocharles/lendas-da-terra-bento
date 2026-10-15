# Lendas da Terra: A Jornada de Bento

Documento de Design de Jogo (GDD) — versão consolidada.

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
| **Dona Nair** | Guia; fala do avô, do amuleto e da receita do Tacacá (oferenda + buff) |
| **Namorada do Boto** | Sente falta do “rapaz bonito” das noites; após purificação: feliz, apaixonada — ele voltou a aparecer |
| **Pescador / testemunha da Iara** | Conta que a Iara protegia pescadores e banhistas; agora, corrompida, ataca e leva ao fundo |

---

## 6. Amuleto — fragmentos

| Lenda | Fragmento | Função |
|---|---|---|
| *(base)* | Visão Espectral | Pegadas, símbolos, auras, pontos fracos de puzzle |
| **Curupira** | Passo Invertido | Ver caminhos falsos; vegetação cresce como plataformas |
| **Boto** | *(a definir — nado/mergulho)* | Travessia aquática |
| **Iara** | Gota de Quartzo Purificador | Mecanismos de água; purificar fluídos; ecos sob a água |
| **Boitatá** | Gema da Chama Fria | Luz no escuro; manipular fogo sem queimar |
| **Saci** | Fragmento do Redemoinho | Vento: mover objetos, cataventos, pressão |
| Cuca / Negrinho / outros | A definir | Capítulos 4–5 |

---

## 7. Culinária e efeitos

| Região | Pratos | Ingredientes | Efeito |
|---|---|---|---|
| **Norte** | Tacacá / Pato no Tucupi | Jambu, Tucupi, Goma de Tapioca | **Treme:** resist. paralisia/veneno; aumenta dano elétrico/espiritual da besta. Tacacá também é **oferenda** a Boto e Iara |
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

1. **Chegada de Barca.** Vila acinzentada: fumaça e incêndio ao fundo, água poluída, pesca e banho proibidos (Barão + corrupção). Dona Nair recebe Bento: avô, amuleto, receita do Tacacá (oferenda a Boto/Iara + buff).
2. **Exploração** com chicote, facão e Visão Espectral; Sombras; momentos de afeto.
3. **Pistas.** Namorada do Boto + pescador da Iara alimentam o Caderno; Mesa de Pesquisa + diário do avô revelam o **Novelo de Cipó** para o Curupira.
4. **Rio: Boto + Iara.** Boto corrompido bloqueia o rio (puzzle); Iara, guardiã secundária, entrega a **Gota de Quartzo** (necessária para a arena). Tacacá como oferenda.
5. **Feira e escambo.** Cipó, resina, jambu, tucupi, goma; fundo de pesquisa com teto; moeda na feira.
6. **Preparo na Barca.** Novelo Encantado, flechas, Tacacá/Pato no Tucupi.
7. **Resgate da fauna** nas armadilhas do Barão.
8. **Ritual do Curupira (híbrido).** Visão Espectral → novelo → purificar focos + Sombras → corrupção a 0%. Forma tradicional (menino, cabelo de fogo, pés invertidos). Recompensa: **Passo Invertido**.
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
- Fragmento exato do **Boto**.

---

## 11. Roteiro técnico da fatia vertical

1. ~~Movimento + chicote (balanço) + pulo duplo~~ ✅
2. ~~Câmera (Cinemachine + limites)~~ ✅
3. ~~Vida, dano, checkpoint, zona de morte~~ ✅
4. UI básica (energia, corrupção) — vida ✅ protótipo
5. ~~Diálogo base + Dona Nair~~ ✅ · falta namorada do Boto e pescador
6. Inventário + economia (escambo, moeda, fundo com teto)
7. Combate (facão, besta, Sombra) + chicote de combate/puzzle
8. Caderno de Campo + Mesa de Pesquisa
9. Visão Espectral
10. Barca (Fogão, Alquimia, salvamento) + receitas Norte
11. Encontros Boto + Iara
12. Resgate da fauna
13. Chefe Curupira + restauração + epílogo
14. Mapa Geral (mínimo: Norte liberado)
15. Arte, áudio, Input System / toque, playtest
