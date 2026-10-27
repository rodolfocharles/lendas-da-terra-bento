# Backlog — Capítulo 1 (Amazônia)

Para colar/espelhar no Notion. Status: `Backlog` | `Em desenvolvimento` | `Concluído`.

## Concluído


| ID  | Tarefa                                                  | Épico      |
| --- | ------------------------------------------------------- | ----------- |
| C01 | Título oficial e estrutura do Cap. 1                   | Produção  |
| C02 | Movimento: andar, pulo variável, coyote, buffer        | Base        |
| C03 | Chicote: balanço em pontos de gancho                   | Base        |
| C04 | Pulo duplo + recarga ao soltar chicote                  | Base        |
| C05 | Camadas Chao/Gancho/Jogador + configurador`Lendas/`     | Base        |
| C06 | GDD consolidado (pilares, Cap. 1, culinária, amuleto)  | Produção  |
| C07 | Câmera Cinemachine + Confiner2D (`LimiteDaFase`)       | Base        |
| C08 | Vida, dano, invencibilidade, checkpoint e zona de morte | Base        |
| C09 | HUD de vida (protótipo OnGUI)                          | Base        |
| C10 | Sistema de diálogo + Dona Nair (placeholder)           | Narrativa   |
| C11 | NPC Lia (Namorada do Boto) — pistas sobre o Boto       | Narrativa   |
| C12 | NPC Seu Zeca (pescador) — pistas sobre a Iara          | Narrativa   |
| C13 | Layout Cap. 1 em faixas (Barca / Vila / Rio / Mata / Ruínas / Arena) | Base |
| C14 | Canoa no cais (chave, remar A/D, 3 impactos afundam)   | Encontros   |
| C15 | HUD energia do amuleto + Essência das Águas            | Base        |
| I01 | Inventário (itens, ingredientes, reagentes)            | Inventário |
| I05 | Ingredientes Norte: Jambu, Tucupi, Goma de Tapioca      | Inventário |
| B10 | Barca navegável (entrar/sair, estações)              | Bases       |
| B11 | Fogão a Lenha — Tacacá (oferenda + buff Treme)       | Bases       |
| B12 | Fogão — Pato no Tucupi                                | Bases       |
| B14 | Sistema de buffs temporários de comida                 | Bases       |
| B13 | Bancada de Alquimia (flechas + Novelo Encantado)        | Bases       |
| X01 | Facão (combo curto + cortar vegetação)               | Combate     |
| X02 | Besta com mira em arco + munição                      | Combate     |
| X03 | Inimigo Sombra (purificação)                          | Combate     |
| X04 | Capanga de treino (vida + chicote)                    | Combate     |
| X07 | Visão Espectral (revelar pistas/plataformas)           | Combate     |
| N05 | Caderno de Campo (páginas, rascunhos, abrir no boss)   | Narrativa   |
| N06 | Diário do avô na Mesa de Pesquisa                     | Narrativa   |
| X05 | Chicote em combate (desarmar, puxar, desequilibrar)     | Combate     |
| X06 | Chicote em puzzle (puxar alavanca)                      | Combate     |
| X08 | 1–2 puzzles estilo Indy (placeholder)                   | Combate     |
| X09 | Layout fase Cap. 1 (blocos: vila, rio, arena)           | Combate     |
| H10 | Encontro Boto: canoa, 3 flechas, Tacacá + foto, essência | Encontros   |
| H11 | Iara na canoa: Treme, pente/espelho/fitas, Gota         | Encontros   |
| H01 | Boss Curupira (ritual novelo + Sombras + corrupção)     | Encontros   |
| H13 | Oferendas Cap. 1: Nair (Tacacá+fitas), Lia (foto+espelho), Zeca (chave+pente) | Narrativa |

## Em desenvolvimento

_(vazio)_

## Backlog (ordem sugerida)

### Épico: Base


| ID  | Tarefa                                                                | Depende de |
| --- | --------------------------------------------------------------------- | ---------- |
| B03 | Unificar barra de corrupção do chefe na HUD persistente | C09, C15, H01 |
| B04 | Salvamento/carregamento (JSON) na Barca                               | B02, B10   |
| B05 | Menu pausa / opções                                                 | B03        |
| B06 | Migrar Input System (teclado, controle, toque)                        | B01        |

### Épico: Narrativa


| ID   | Tarefa                                                                     | Depende de           |
| ---- | -------------------------------------------------------------------------- | -------------------- |
| N02  | Expandir falas Dona Nair + retratos reais                                  | C10                  |
| N03b | Epílogo da Lia (feliz/apaixonada após purificar Boto)                    | C11, H10             |
| N04b | Epílogo do Seu Zeca após purificar Iara                                  | C12, H11             |
| N07  | Sequência pós-restauração (Dona Nair, crianças, pescadores, namorada) | N02, N03b, N04b, H01 |

### Épico: Inventário e economia


| ID  | Tarefa                              | Depende de |
| --- | ----------------------------------- | ---------- |
| I02 | Escambo com NPCs                    | I01, C10   |
| I03 | Feira / mercador com moeda          | I01        |
| I04 | Fundo de pesquisa com teto por fase | I03        |

### Épico: Combate e exploração


| ID  | Tarefa                                        | Depende de |
| --- | --------------------------------------------- | ---------- |
| X04b | Máquina / drone do Barão (inimigo industrial) | X04 |

### Épico: Bases e culinária

_(Fogão, Alquimia e Mesa de Pesquisa no protótipo)_

### Épico: Encontros Cap. 1


| ID  | Tarefa                                                | Depende de         |
| --- | ----------------------------------------------------- | ------------------ |
| H12 | Resgate da fauna                                      | X09, I01           |
| H02 | Restauração do belo (cor + música)                 | H01                |
| H03 | Habilidade Passo Invertido (caminhos falsos / vegetação) | H01             |
| H04 | Mapa Geral do Brasil (Norte liberado; revisitar)      | H02                |

### Épico: Apresentação


| ID  | Tarefa                                                    | Depende de |
| --- | --------------------------------------------------------- | ---------- |
| A01 | Placeholder → primeira arte Bento / Dona Nair / cenário | —         |
| A02 | Animações base (andar, pulo, chicote, chapéu, alívio) | A01        |
| A03 | Música Cap. 1 (carimbó) + SFX                           | H02        |
| A04 | Build PC + Android smoke test                             | B06        |
| A05 | Playtest da fatia vertical                                | H02        |

## Epicós Notion (propriedades sugeridas)

- **Status:** Backlog / Em desenvolvimento / Concluído
- **Épico:** Base | Narrativa | Inventário | Combate | Bases | Encontros | Apresentação | Produção
- **Prioridade:** P0 (fatia vertical) / P1 / P2
- **Depende de:** relação entre tarefas
