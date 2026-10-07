using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan.Entitlement;

public enum LimitUnit
{
	Video = 1,
	// Hajime - 初め
	// 3 videos / total
	//
	// Manabu - 学ぶ
	// 5 videos / day
	//
	// Jōtatsu - 上達
	// 10 videos / day


	Chat = 2,
	// Hajime - 初め
	// 30 chats / total
	//
	// Manabu - 学ぶ
	// 100 chats / day
	//
	// Jōtatsu - 上達
	// 500 chats / day


	Quiz = 3,
	// Hajime - 初め
	// 3 quizzes / total
	//
	// Manabu - 学ぶ
	// 5 quizzes / day
	//
	// Jōtatsu - 上達
	// 10 quizzes / day


	Character = 4,
	// Hajime - 初め
	// 1 characters / total
	//
	// Manabu - 学ぶ
	// 5 characters / day
	//
	// Jōtatsu - 上達
	// 10 characters / day
}