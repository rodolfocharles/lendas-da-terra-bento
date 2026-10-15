# Backlog — Capítulo 1 (Amazônia)

Para colar/espelhar no Notion. Status: `Backlog` | `Em desenvolvimento` | `Concluído`.

## Concluído

| ID | Tarefa | Épico |
|---|---|---|
| C01 | Título oficial e estrutura do Cap. 1 | Produção |
| C02 | Movimento: andar, pulo variável, coyote, buffer | Base |
| C03 | Chicote: balanço em pontos de gancho | Base |
| C04 | Pulo duplo + recarga ao soltar chicote | Base |
| C05 | Camadas Chao/Gancho/Jogador + configurador `Lendas/` | Base |
| C06 | GDD consolidado (pilares, Cap. 1, culinária, amuleto) | Produção |
| C07 | Câmera Cinemachine + Confiner2D (`LimiteDaFase`) | Base |
| C08 | Vida, dano, invencibilidade, checkpoint e zona de morte | Base |
| C09 | HUD de vida (protótipo OnGUI) | Base |
| C10 | Sistema de diálogo + Dona Nair (placeholder) | Narrativa |

## Em desenvolvimento

_(vazio)_

## Backlog (ordem sugerida)

### Épico: Base
| ID | Tarefa | Depende de |
|---|---|---|
| B03 | UI HUD completa (energia espiritual, munição, barra de corrupção) | C09 |
| B04 | Salvamento/carregamento (JSON) na Barca | B02, B10 |
| B05 | Menu pausa / opções | B03 |
| B06 | Migrar Input System (teclado, controle, toque) | B01 |

### Épico: Narrativa
| ID | Tarefa | Depende de |
|---|---|---|
| N02 | Expandir falas Dona Nair + retratos reais | C10 |
| N03 | NPC Namorada do Boto (pista + epílogo feliz/apaixonada) | C10 |
| N04 | NPC Pescador / testemunha da Iara | C10 |
| N05 | Caderno de Campo (páginas, rascunhos, abrir no boss) | C10, I01 |
| N06 | Diário do avô na Mesa de Pesquisa | N05, B10 |
| N07 | Sequência pós-restauração (Dona Nair, crianças, pescadores, namorada) | N02–N04, H01 |

### Épico: Inventário e economia
| ID | Tarefa | Depende de |
|---|---|---|
| I01 | Inventário (itens, ingredientes, reagentes) | — |
| I02 | Escambo com NPCs | I01, N01 |
| I03 | Feira / mercador com moeda | I01 |
| I04 | Fundo de pesquisa com teto por fase | I03 |
| I05 | Ingredientes Norte: Jambu, Tucupi, Goma de Tapioca | I01 |

### Épico: Combate e exploração
| ID | Tarefa | Depende de |
|---|---|---|
| X01 | Facão (combo curto + cortar vegetação) | C02 |
| X02 | Besta com mira em arco + munição | C02 |
| X03 | Inimigo Sombra (purificação) | X02, B03 |
| X04 | Capanga / máquina do Barão (vida) | X01 |
| X05 | Chicote em combate (desarmar, puxar, desequilibrar) | C03, X01 |
| X06 | Chicote em puzzle (puxar alavanca) | C03 |
| X07 | Visão Espectral (revelar pistas/plataformas) | — |
| X08 | 1–2 puzzles estilo Indy (placeholder) | X06, X07 |
| X09 | Layout fase Cap. 1 (blocos: vila, rio, arena) | B01 |

### Épico: Bases e culinária
| ID | Tarefa | Depende de |
|---|---|---|
| B10 | Barca navegável (entrar/sair, estações) | B01 |
| B11 | Fogão a Lenha — Tacacá (oferenda + buff Treme) | B10, I05, N02 |
| B12 | Fogão — Pato no Tucupi | B11 |
| B13 | Bancada de Alquimia (flechas + Novelo Encantado) | B10, I01 |
| B14 | Sistema de buffs temporários de comida | B11, B02 |

### Épico: Encontros Cap. 1
| ID | Tarefa | Depende de |
|---|---|---|
| H10 | Puzzle / encontro Boto (rio bloqueado) | X09, N03, B11 |
| H11 | Encontro Iara + Gota de Quartzo | H10, N04 |
| H12 | Resgate da fauna | X09, I01 |
| H01 | Boss Curupira (ritual novelo + Sombras + corrupção) | H11, B13, N05, X03 |
| H02 | Restauração do belo (cor + música) | H01 |
| H03 | Fragmento Passo Invertido no amuleto | H01 |
| H04 | Mapa Geral do Brasil (Norte liberado; revisitar) | H02 |

### Épico: Apresentação
| ID | Tarefa | Depende de |
|---|---|---|
| A01 | Placeholder → primeira arte Bento / Dona Nair / cenário | — |
| A02 | Animações base (andar, pulo, chicote, chapéu, alívio) | A01 |
| A03 | Música Cap. 1 (carimbó) + SFX | H02 |
| A04 | Build PC + Android smoke test | B06 |
| A05 | Playtest da fatia vertical | H02 |

## Epicós Notion (propriedades sugeridas)

- **Status:** Backlog / Em desenvolvimento / Concluído  
- **Épico:** Base | Narrativa | Inventário | Combate | Bases | Encontros | Apresentação | Produção  
- **Prioridade:** P0 (fatia vertical) / P1 / P2  
- **Depende de:** relação entre tarefas  
