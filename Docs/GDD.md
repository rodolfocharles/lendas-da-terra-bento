# Lendas da Terra: A Jornada de Bento

Documento de Design de Jogo (GDD) — versão revisada.

## 1. Visão geral

- **Gênero:** 2D Action-Adventure / Puzzle RPG side-scroller com progressão estilo metroidvania leve.
- **Engine:** Unity 6 (URP 2D), C#.
- **Plataformas:** PC, celular (Android/iOS) e web; consoles como meta futura.
- **Estilo visual:** aquarela 2D feita à mão, contornos suaves, inspiração em Spiritfarer.
- **Tom:** empático, cultural, aventureiro, defensor do patrimônio histórico e ambiental.
- **Idiomas:** português (principal) e inglês.

### Pilar central
As lendas não são vilãs: foram corrompidas pelo Barão. Bento **purifica** em vez de matar. Inimigos espirituais têm barra de **corrupção (100% → 0%)**; inimigos industriais (capangas, máquinas) têm vida comum.

## 2. Protagonista: Bento

Etnógrafo, arqueólogo e folclorista. Chapéu, mochila, chicote enrolado e amuleto no peito.

**Movimento base:** correr, pulo variável, pulo duplo (`pulosExtrasMax`, recarrega no chão e ao soltar o chicote), coyote time e buffer de pulo.

| Ferramenta | Função |
|---|---|
| Chicote / corda com gancho | Balançar em pontos de gancho, alcançar plataformas, desarmar mecanismos |
| Facão de campo | Abrir vegetação, combate corpo a corpo |
| Besta Espiritual | Disparo em arco (trajetória 2D visível), gatilhos à distância, flechas elementais |
| Amuleto Ancestral | Visão Espectral (revela pistas, plataformas e espíritos); recebe as bênçãos das lendas |
| Caderno de Campo | Mapa, bestiário, receitas, pistas e registros culturais |

## 3. Antagonista

**Barão Valério da Rocha** — magnata do extrativismo ilegal. Usa magia negra para corromper os guardiões e culpar as lendas pela destruição. Cada capítulo tem uma **Fortaleza Extrativista** do Barão.

### Inimigos
- **Sombras:** espíritos básicos da corrupção (roxos), purificáveis.
- **Capangas e máquinas do Barão:** inimigos industriais, perto das fortalezas.

## 4. Bases móveis

- **Barca de Rio:** Amazônia, São Francisco, Pantanal.
- **Kombi de Expedição:** estradas do Sertão, Sudeste e Sul.

Estações internas: **Fogão a Lenha** (pratos com bônus), **Bancada de Alquimia** (munições elementais), **Mesa de Pesquisa** (relíquias e Caderno). A base é ponto de salvamento.

## 5. Diálogos

- Caixa de texto com retrato desenhado à mão.
- Cada fala tem: personagem, texto e **emoção** (troca o retrato). Sem escolhas de resposta.
- Cada região tem **um guia próprio** (NPC regional).

## 6. Capítulos

| Cap. | Região | Chefe | Guardião secundário | Base | Prato (bônus) | Música |
|---|---|---|---|---|---|---|
| 1 | Norte (Amazônia) | Curupira | Boto-cor-de-rosa | Barca | Tacacá (resistência à paralisia) | Carimbó, flautas, percussão |
| 2 | Nordeste (Salvador e Sertão) | Cabeça de Cuia | Saci, Boitatá | Kombi e Barca | Moqueca (resist. fogo espiritual), Baião de Dois | Samba de roda, Olodum, forró |
| 3 | Centro-Oeste (Pantanal e Cerrado) | A definir (Mãe do Ouro / Pé de Garrafa) | A definir | Kombi e Barca | Galinhada com pequi | Moda de viola |
| 4 | Sudeste (Rio e cidades históricas) | Cuca | Mula sem Cabeça | Kombi | Feijoada | Choro, bossa nova, samba |
| 5 | Sul (Pampas e Serras) | Negrinho do Pastoreio | Mboi Tu'i | Kombi | Barreado, chimarrão | Chamamé, milonga |

### Habilidades concedidas pelas lendas (proposta)
| Lenda | Habilidade |
|---|---|
| Curupira | Rastreio: revela pegadas e caminhos ocultos |
| Boto | Nado / mergulho |
| Saci | Redemoinho: impulso aéreo (dash) |
| Boitatá | Fogo: queima obstáculos, flechas incendiárias |
| Cuca | Sono/ilusão: congela inimigos, plataformas temporárias |
| Negrinho do Pastoreio | Vela: Visão Espectral aprimorada (acha o que está perdido) |

## 7. Capítulo 1 — O Clamor dos Igarapés (fatia vertical)

- **Cenário:** igarapés, palafitas, cerâmica marajoara, grafismos indígenas, Fortaleza do Barão ao fundo.
- **Chefe Curupira (forma tradicional):** menino de cabelo de fogo e pés virados para trás, com aura roxa de corrupção.
  - Luta de agilidade e leitura: pegadas falsas, clones de sombra, assobio paralisante.
  - Visão Espectral identifica o verdadeiro; a besta purifica à distância.
  - Fases por faixa de corrupção (100–66%, 66–33%, 33–0%).
  - Recompensa: habilidade **Rastreio**.
- **Boto-cor-de-rosa:** encontro no rio perto da Barca; recompensa: nado.
- **Guia regional:** a definir (nome e papel).

## 8. Diretrizes de arte e cultura

- Imagens geradas por IA servem só de **referência/conceito**; assets finais precisam de consistência e animação.
- Pesquisar e creditar referências indígenas e tradicionais; evitar estereótipos.
- Na Steam, declarar uso de IA se houver conteúdo gerado no jogo final.

## 9. Roteiro da fatia vertical

1. **Movimento** — correr, pular (coyote time, buffer), chicote com balanço. ✅ em andamento
2. **Câmera** — Cinemachine seguindo o Bento com limites da fase.
3. **Combate** — facão, besta com mira em arco, Sombra e capanga.
4. **Amuleto** — Visão Espectral revelando plataformas e pistas.
5. **Narrativa** — diálogo com retrato/emoção, Caderno de Campo básico.
6. **Barca** — Tacacá, alquimia de flechas, salvamento.
7. **Chefe Curupira** — fases, purificação, habilidade Rastreio.
8. **Polimento** — primeira arte, música, controles de toque.
